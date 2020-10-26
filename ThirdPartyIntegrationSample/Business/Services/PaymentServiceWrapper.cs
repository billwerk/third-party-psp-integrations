using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Billwerk.Payment.PayOne.Model;
using Billwerk.Payment.PayOne.Services;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Billwerk.Payment.SDK.Enums.ExternalIntegration;
using Billwerk.Payment.SDK.Interfaces;
using Business.Enums;
using Business.Helpers;
using Business.Interfaces;
using MongoDB.Bson;
using Business.Models;
using Business.PayOne.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Business.Services
{
    public class PaymentServiceWrapper : IPaymentServiceWrapper
    {
        private const string NotFoundErrorMessage = "Not Found";
        private const string InvalidPreconditionsErrorMessage = "Transaction Id is empty";
        private const string RefundAction = "refund";

        private readonly PayOnePaymentService _payOnePaymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IRecurringTokenService _recurringTokenService;
        private readonly IRecurringTokenEncoder<RecurringToken> _recurringTokenEncoder;
        private readonly ILogger<PaymentServiceWrapper> _logger;

        public PaymentServiceWrapper(PayOnePaymentService payOnePaymentService, IPaymentTransactionService paymentTransactionService,
            IRecurringTokenService recurringTokenService, IRecurringTokenEncoder<RecurringToken> recurringTokenEncoder,
            ILogger<PaymentServiceWrapper> logger)
        {
            _payOnePaymentService = payOnePaymentService;
            _paymentTransactionService = paymentTransactionService;
            _recurringTokenService = recurringTokenService;
            _recurringTokenEncoder = recurringTokenEncoder;
            _logger = logger;
        }

        private IPaymentService GetPaymentService(PaymentServiceProvider provider)
        {
            switch (provider)
            {
                case PaymentServiceProvider.PayOne:
                    return _payOnePaymentService;
                default:
                    throw new NotSupportedException($"Provide={provider} is not supported!");
            }            
        }
        
        public async Task<ExternalPaymentTransactionDTO> SendPayment(PaymentServiceProvider provider, ExternalPaymentRequestDTO paymentDto)
        {
            var externalPaymentRequest = new ExternalPaymentRequestWrapperDTO
            {
                PaymentRequestDto = paymentDto
            };

            var resultOfPopulation = TryToPopulatePreauthRequestDto(externalPaymentRequest, out var sequenceNumber);
            if (resultOfPopulation != null) return resultOfPopulation;

            resultOfPopulation = TryToPopulateRecurringToken(externalPaymentRequest.PaymentRequestDto);
            if (resultOfPopulation != null) return resultOfPopulation;

            var paymentResult = await GetPaymentService(provider).SendPayment(externalPaymentRequest);

            var mappedPaymentTransaction = paymentResult.PaymentDto.ToEntity();
            mappedPaymentTransaction.SequenceNumber = sequenceNumber;
            mappedPaymentTransaction.MerchantSettings = paymentDto.MerchantSettings;
            mappedPaymentTransaction.Role = paymentDto.PaymentMeansReference.Role;
            mappedPaymentTransaction.WebhookTarget = paymentDto.WebhookTarget;
            mappedPaymentTransaction.InvoiceReferenceCode = paymentDto.InvoiceReferenceCode;
            mappedPaymentTransaction.TransactionReferenceText = paymentDto.TransactionReferenceText;
            mappedPaymentTransaction.TransactionInvoiceReferenceText = paymentDto.TransactionInvoiceReferenceText;

            paymentResult.PaymentDto.ExternalTransactionId = mappedPaymentTransaction.Id.ToString();

            _paymentTransactionService.Create(mappedPaymentTransaction);

            TransformAndUpdateRecurringToken(paymentResult.RecurringToken);

            return paymentResult.PaymentDto;
        }

        public async Task<ExternalRefundTransactionDTO> SendRefund(PaymentServiceProvider provider, string transactionId, ExternalRefundRequestDTO dto)
        {
            var targetTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId) as PaymentTransaction;
            if (targetTransaction == null)
            {
                return new ExternalRefundTransactionDTO
                {
                    Error = CreateUnmappedError()
                };
            }

            var refundResult = await GetPaymentService(provider).SendRefund(dto, targetTransaction);

            if (refundResult.Status == PaymentTransactionNewStatus.Succeeded)
            {
                _paymentTransactionService.UpdateTransactionSeqNumber(targetTransaction, targetTransaction.SequenceNumber + 1);
            }

            var refundTransaction = refundResult.ToEntity();

            refundTransaction.PaymentTransactionId = targetTransaction.Id.AsTyped<PaymentTransaction>();
            refundTransaction.MerchantSettings = dto.MerchantSettings;
            refundTransaction.Role = targetTransaction.Role;
            refundTransaction.WebhookTarget = dto.WebhookTarget;
            refundTransaction.SequenceNumber = targetTransaction.SequenceNumber;

            refundResult.ExternalTransactionId = refundTransaction.Id.ToString();

            _paymentTransactionService.Create(refundTransaction);

            return refundResult;
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(PaymentServiceProvider provider, ExternalPreauthRequestDTO dto)
        {
            var resultOfPopulation = TryToPopulateRecurringToken(dto);
            if (resultOfPopulation != null)
                return new ExternalPreauthTransactionDTO
                {
                    Error = resultOfPopulation.Error
                };

            var preauthResult = await GetPaymentService(provider).SendPreauth(dto);

            preauthResult.RecurringToken = TransformAndUpdateRecurringToken(preauthResult.RecurringToken);

            var preauthTransaction = preauthResult.ToEntity();

            preauthTransaction.SequenceNumber = 0;
            preauthTransaction.MerchantSettings = dto.MerchantSettings;
            preauthTransaction.Role = dto.PaymentMeansReference.Role;
            preauthTransaction.WebhookTarget = dto.WebhookTarget;

            preauthResult.ExternalTransactionId = preauthTransaction.Id.ToString();

            _paymentTransactionService.Create(preauthTransaction);

            return preauthResult;
        }

        public Task<ExternalPaymentTransactionDTO> FetchPayment(PaymentServiceProvider provider, string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                return Task.FromResult(new ExternalPaymentTransactionDTO
                {
                    Error = CreateInvalidPreconditionsError()
                });
            }

            var transaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId);
            if (transaction != null && transaction is PaymentTransaction paymentTransaction)
            {
                return Task.FromResult(paymentTransaction.ToDto());
            }

            return Task.FromResult(new ExternalPaymentTransactionDTO
            {
                Error = CreateUnmappedError()
            });
        }

        public Task<ExternalRefundTransactionDTO> FetchRefund(PaymentServiceProvider provider, string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                return Task.FromResult(new ExternalRefundTransactionDTO
                {
                    Error = CreateInvalidPreconditionsError()
                });
            }

            var transaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId);
            if (transaction != null && transaction is RefundTransaction refundTransaction)
            {
                return Task.FromResult(refundTransaction.ToDto());
            }

            return Task.FromResult(new ExternalRefundTransactionDTO
            {
                Error = CreateUnmappedError()
            });
        }

        public Task<ExternalPreauthTransactionDTO> FetchPreauth(PaymentServiceProvider provider, string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                return Task.FromResult(new ExternalPreauthTransactionDTO
                {
                    Error = CreateInvalidPreconditionsError()
                });
            }

            var transaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId);
            if (transaction != null && transaction is PreauthTransaction preauthTransaction)
            {
                return Task.FromResult(preauthTransaction.ToDto());
            }

            return Task.FromResult(new ExternalPreauthTransactionDTO
            {
                Error = CreateUnmappedError()
            });
        }

        public async Task<ExternalPaymentCancellationDTO> SendCancellation(PaymentServiceProvider provider, string transactionId)
        {
            var paymentTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId);
            if (paymentTransaction != null && paymentTransaction is PreauthTransaction preauthTransaction)
            {
                return await GetPaymentService(provider).SendCancellation(preauthTransaction.ToRequestDto(), preauthTransaction);
            }

            return new ExternalPaymentCancellationDTO
            {
                Error = new ExternalIntegrationErrorDTO
                {
                    ErrorMessage = $"The transaction with id {transactionId} hasn't been found."
                }
            };
        }

        public ObjectResult HandleWebhookAsync(string requestString)
        {
            var result = requestString.Replace("\n", string.Empty);
            try
            {
                var ts = new TransactionStatus(result);

                _logger.LogDebug($"PayOne: provider transaction id {ts.TxId}, status {ts.TxAction}");

                if (TryToIdentifyTransaction(ts, out var paymentTransaction, out var buildAcceptResult))
                    return buildAcceptResult;

                MapPaymentTransactionStatus(ts, paymentTransaction, out var wasSkipped);

                if (AnalyzeSequenceNumber(ts, paymentTransaction, wasSkipped, out var objectResult)) return objectResult;

                _paymentTransactionService.Update(paymentTransaction);

                BackgroundJob.Enqueue<IWebhookService>(service =>
                    service.Send(paymentTransaction.WebhookTarget, paymentTransaction.ExternalTransactionId));

                return BuildAcceptResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process PayOne webhook: {result}", ex);

                return new InternalServerErrorMessageResult("Could not process event");
            }
        }

        private bool AnalyzeSequenceNumber(TransactionStatus ts, PaymentTransactionBase paymentTransaction, bool wasSkipped,
            out ObjectResult objectResult)
        {
            var wasUpdated = false;
            objectResult = null;

            if (!int.TryParse(ts.Sequencenumber, out var sequenceNumber))
            {
                return false;
            }

            if (sequenceNumber > paymentTransaction.SequenceNumber)
            {
                wasUpdated = _paymentTransactionService.UpdateTransactionSeqNumber(paymentTransaction, sequenceNumber);
                if (!wasUpdated)
                {
                    _logger.LogError(
                        $"Updating SequenceNumber to {sequenceNumber} for transaction {paymentTransaction.Id} failed");
                }
            }

            if (!wasSkipped)
            {
                var successOperations = paymentTransaction.SequenceNumber;
                if (sequenceNumber + 1 < successOperations)
                {
                    _logger.LogDebug($"Skip webhook for transactionId={paymentTransaction.Id} because it's old");

                    {
                        objectResult = BuildAcceptResult();
                        return true;
                    }
                }
            }
            else if (!wasUpdated)
            {
                _logger.LogDebug(
                    $"Skip webhook for transactionId={paymentTransaction.Id} because of irrelevant webhook action and irrelevant sequence number");

                {
                    objectResult = BuildAcceptResult();
                    return true;
                }
            }

            return false;
        }

        private bool TryToIdentifyTransaction(TransactionStatus ts, out PaymentTransactionBase paymentTransaction,
            out ObjectResult buildAcceptResult)
        {
            buildAcceptResult = null;

            var pspTransaction = _paymentTransactionService.SinglePspTransactionByProviderTransactionId(ts.TxId);
            if (pspTransaction == null)
            {
                paymentTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(ts.Param);

                if (paymentTransaction == null)
                {
                    _logger.LogWarning(
                        $"PayOne: provider transaction not found (id={ts.TxId},our reference={ts.Param}). Probably it is a transaction of a different system");

                    {
                        buildAcceptResult = BuildAcceptResult();
                        return true;
                    }
                }

                if (paymentTransaction.GetType() == typeof(PreauthTransaction))
                {
                    var captureTransaction =
                        _paymentTransactionService.SingleByPreauthTransactionId((paymentTransaction as PreauthTransaction)
                            .GetId());
                    if (captureTransaction != null) paymentTransaction = captureTransaction;
                }
            }
            else
            {
                var referencedTransaction = pspTransaction.GetByExternalTransactionId(ts.Param);
                paymentTransaction = ts.TxAction == RefundAction
                    ? pspTransaction.GetRefundTransaction(int.Parse(ts.Sequencenumber))
                    : pspTransaction.GetLatest();

                //Todo: if Action is refund and transaction == null => External Refund

                if (referencedTransaction == null || paymentTransaction == null)
                {
                    _logger.LogWarning($"PayOne: Webhook for transaction {ts.Param} is invalid. Rejecting request");

                    {
                        buildAcceptResult = new BadRequestObjectResult(string.Empty);
                        return true;
                    }
                }
            }

            return false;
        }

        private static OkObjectResult BuildAcceptResult()
        {
            return new OkObjectResult(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
        }

        #region private methods

        private static ExternalIntegrationErrorDTO CreateUnmappedError()
        {
            return new ExternalIntegrationErrorDTO
            {
                ErrorCode = PaymentErrorCode.UnmappedError,
                ErrorMessage = NotFoundErrorMessage
            };
        }

        private static ExternalIntegrationErrorDTO CreateInvalidPreconditionsError()
        {
            return new ExternalIntegrationErrorDTO
            {
                ErrorCode = PaymentErrorCode.InvalidPreconditions,
                ErrorMessage = InvalidPreconditionsErrorMessage
            };
        }

        private ExternalPaymentTransactionDTO TryToPopulateRecurringToken(
            ExternalPaymentTransactionBasePaymentRequestDTO requestDto)
        {
            var externalRecurringToken = requestDto.PaymentMeansReference.RecurringToken;
            if (string.IsNullOrWhiteSpace(externalRecurringToken)) return null;

            var recurringToken = _recurringTokenService.SingleByIdOrDefault(ObjectId.Parse(externalRecurringToken));
            if (recurringToken == null)
            {
                return new ExternalPaymentTransactionDTO
                {
                    Error = new ExternalIntegrationErrorDTO
                    {
                        ErrorCode = PaymentErrorCode.InvalidPreconditions,
                        ErrorMessage = $"Unknown recurringToken {externalRecurringToken}"
                    }
                };
            }

            requestDto.PaymentMeansReference.RecurringToken = _recurringTokenEncoder.Encrypt(recurringToken);

            return null;
        }

        private ExternalPaymentTransactionDTO TryToPopulatePreauthRequestDto(ExternalPaymentRequestWrapperDTO externalPaymentRequest,
            out int sequenceNumber)
        {
            sequenceNumber = 0;

            var externalPreauthTransactionId =
                externalPaymentRequest?.PaymentRequestDto?.PaymentMeansReference?.PreauthTransactionId;
            if (string.IsNullOrWhiteSpace(externalPreauthTransactionId)) return null;

            var paymentTransaction =
                _paymentTransactionService.SingleByExternalTransactionIdOrDefault(externalPreauthTransactionId);

            if (paymentTransaction == null || !(paymentTransaction is PreauthTransaction preauthTransaction))
                return new ExternalPaymentTransactionDTO
                {
                    Error = new ExternalIntegrationErrorDTO
                    {
                        ErrorCode = PaymentErrorCode.InvalidPreconditions,
                        ErrorMessage = $"Unknown preauthTransactionId {externalPreauthTransactionId}"
                    }
                };

            externalPaymentRequest.PreauthRequestDto = preauthTransaction.ToRequestDto();
            externalPaymentRequest.BearerDto = preauthTransaction.Bearer;
            externalPaymentRequest.PspTransactionId = preauthTransaction.PspTransactionId;

            sequenceNumber = 1;

            return null;
        }

        private string TransformAndUpdateRecurringToken(string recurringTokenHash)
        {
            if (string.IsNullOrWhiteSpace(recurringTokenHash))
            {
                return null;
            }

            var recurringToken = _recurringTokenEncoder.Decrypt(recurringTokenHash);
            if (recurringToken.Id == ObjectId.Empty)
            {
                _recurringTokenService.Create(recurringToken);
            }
            else
            {
                _recurringTokenService.Update(recurringToken);
            }

            return recurringToken.Id.ToString();
        }

        private static decimal GetChargebackFeeAmount(TransactionStatus status, PaymentTransaction transaction)
        {
            var receivable = decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
            var fee = receivable - (transaction.RequestedAmount - transaction.RefundedAmount);

            if ((status.TxAction == "cancelation" || status.TxAction == "debit") && fee > 0m)
            {
                return fee;
            }

            return 0;
        }

        private void MapPaymentTransactionStatus(TransactionStatus status, PaymentTransactionBase transaction,
            out bool wasSkipped)
        {
            decimal receivable = 0;
            decimal balance = 0;
            wasSkipped = false;

            if (string.IsNullOrWhiteSpace(status.Receivable))
            {
                _logger.LogWarning("PayOne: Webhook data does not contain receivable value.");
            }
            else
            {
                receivable = decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrWhiteSpace(status.Balance))
            {
                _logger.LogWarning("PayOne: Webhook data does not contain balance value.");
            }
            else
            {
                balance = decimal.Parse(status.Balance, CultureInfo.InvariantCulture);
            }

            var paymentTransaction = transaction as PaymentTransaction;
            var refundTransaction = transaction as RefundTransaction;
            switch (status.TxAction)
            {
                case "appointed":
                case "capture":
                    wasSkipped = true;
                    // This status should not be relevant for any transaction. Initial Succeeded or PreliminarySucceeded
                    // is set during actual payment request already so this is redundant
                    break;
                case "paid":
                case "underpaid":
                    if (paymentTransaction != null)
                    {
                        var chargebacksFee = paymentTransaction.Chargebacks?.Sum(c => c.FeeAmount) ?? 0;
                        var chargebacksAmount = paymentTransaction.Chargebacks?.Sum(c => c.Amount) ?? 0;
                        var paidAmount = paymentTransaction.Payments?.Sum(c => c.Amount) ?? 0;
                        var targetBalance = paymentTransaction.RequestedAmount + chargebacksFee + chargebacksAmount -
                                            paidAmount;

                        RegisterPayment(status, paymentTransaction, targetBalance - balance);
                    }
                    else
                    {
                        wasSkipped = true;
                    }

                    break;
                case "refund":
                    if (refundTransaction != null)
                    {
                        paymentTransaction =
                            _paymentTransactionService.SingleByIdOrDefault(refundTransaction.PaymentTransactionId.Untyped) as
                                PaymentTransaction;
                        if (paymentTransaction != null)
                        {
                            var chargebacksFee = paymentTransaction.Chargebacks?.Sum(c => c.FeeAmount) ?? 0;
                            var refundedAmount = paymentTransaction.RequestedAmount + chargebacksFee -
                                                 paymentTransaction.RefundedAmount - receivable;
                            if (int.TryParse(status.Sequencenumber, out var sequenceNumber))
                            {
                                // Ignore webhook, because this status is already handled
                                if (refundTransaction.Refunds != null &&
                                    refundTransaction.Refunds.Any(s => s.ExternalItemId == sequenceNumber.ToString()))
                                {
                                    wasSkipped = true;
                                    return;
                                }

                                RegisterRefund(sequenceNumber, refundedAmount, refundTransaction);

                                if (refundTransaction.Refunds != null)
                                {
                                    paymentTransaction.RefundedAmount += refundTransaction.Refunds.Sum(r => r.Amount);
                                    _paymentTransactionService.Update(paymentTransaction);
                                }
                            }
                        }
                        else
                        {
                            wasSkipped = true;
                        }
                    }
                    else
                    {
                        wasSkipped = true;
                    }

                    break;
                case "cancelation":
                    if (paymentTransaction != null)
                    {
                        RegisterChargeback(status, paymentTransaction, paymentTransaction.RequestedAmount);
                    }
                    else
                    {
                        wasSkipped = true;
                    }

                    break;
                default:
                    wasSkipped = true;
                    return;
            }
        }

        private static void RegisterRefund(int sequenceNumber, decimal refundedAmount, RefundTransaction refundTransaction)
        {
            var now = DateTime.Now;
            var externalRefundItemDTO = new ExternalRefundItemDTO
            {
                ExternalItemId = sequenceNumber.ToString(),
                Amount = refundedAmount,
                BookingDate = new LocalDate(now.Year, now.Month, now.Day)
            };
            refundTransaction.Refunds ??= new List<ExternalRefundItemDTO>();
            refundTransaction.Refunds.Add(externalRefundItemDTO);
        }

        private static void RegisterPayment(TransactionStatus status, PaymentTransaction paymentTransaction, decimal amount)
        {
            paymentTransaction.Payments ??= new List<ExternalPaymentItemDTO>();
            var now = DateTime.Now;
            paymentTransaction.Payments.Add(new ExternalPaymentItemDTO
            {
                ExternalItemId = status.Sequencenumber,
                Amount = amount,
                BookingDate = new LocalDate(now.Year, now.Month, now.Day)
            });
        }

        private static void RegisterChargeback(TransactionStatus status, PaymentTransaction paymentTransaction, decimal amount)
        {
            paymentTransaction.Chargebacks ??= new List<ExternalPaymentChargebackItemDTO>();
            var now = DateTime.Now;
            var externalPaymentChargebackItemDTO = new ExternalPaymentChargebackItemDTO
            {
                Amount = amount,
                BookingDate = new LocalDate(now.Year, now.Month, now.Day),
                PspReasonCode = status.FailedCause,
                FeeAmount = GetChargebackFeeAmount(status, paymentTransaction)
            };

            if (string.IsNullOrWhiteSpace(status.FailedCause) == false)
            {
                var failedCause = status.FailedCause.ToLower();

                switch (failedCause)
                {
                    case "soc":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.InsufficientBalance;
                        externalPaymentChargebackItemDTO.PspReasonMessage = "Insufficient funds";
                        break;
                    case "cka":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.BearerInvalid;
                        externalPaymentChargebackItemDTO.PspReasonMessage = "Account expired";
                        break;
                    case "uan":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.BearerInvalid;
                        externalPaymentChargebackItemDTO.PspReasonMessage =
                            "Account no. / name not identical, incorrect or savings account";
                        break;
                    case "ndd":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.BearerInvalid;
                        externalPaymentChargebackItemDTO.PspReasonMessage = "No direct debit";
                        break;
                    case "cb":
                    case "obj":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.Canceled;
                        externalPaymentChargebackItemDTO.PspReasonMessage =
                            "Objection: The payer objects to the direct debit.";
                        break;
                    case "cbn":
                    case "cbk":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.Canceled;
                        externalPaymentChargebackItemDTO.PspReasonMessage = "Credit card chargeback";
                        break;
                    case "ret":
                    case "nelv":
                    case "ncc":
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.Rejected;
                        externalPaymentChargebackItemDTO.PspReasonMessage = "cannot be collected";
                        break;
                    default:
                        externalPaymentChargebackItemDTO.Reason = ExternalPaymentChargebackReason.Unknown;
                        paymentTransaction.StatusHistory.Add(PaymentTransactionNewStatus.Failed);
                        break;
                }

                if (paymentTransaction.Status == PaymentTransactionNewStatus.Failed) return;

                
            }

            paymentTransaction.Chargebacks.Add(externalPaymentChargebackItemDTO);
        }

        #endregion private methods
    }
}
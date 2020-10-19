using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Business.Helpers;
using Business.Interfaces;
using Business.PayOne.Model;
using MongoDB.Bson;
using Business.Models;
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

        private readonly IPaymentService _paymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IRecurringTokenService _recurringTokenService;
        private readonly IRecurringTokenEncoder<RecurringToken> _recurringTokenEncoder;
        private readonly ILogger<PaymentServiceWrapper> _logger;

        public PaymentServiceWrapper(IPaymentService paymentService, IPaymentTransactionService paymentTransactionService,
            IRecurringTokenService recurringTokenService, IRecurringTokenEncoder<RecurringToken> recurringTokenEncoder,
            ILogger<PaymentServiceWrapper> logger)
        {
            _paymentService = paymentService;
            _paymentTransactionService = paymentTransactionService;
            _recurringTokenService = recurringTokenService;
            _recurringTokenEncoder = recurringTokenEncoder;
            _logger = logger;
        }

        public async Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO paymentDto)
        {
            var externalPaymentRequest = new ExternalPaymentRequest
            {
                PaymentRequestDto = paymentDto
            };

            var resultOfPopulation = TryToPopulatePreauthRequestDto(externalPaymentRequest, out var sequenceNumber);
            if (resultOfPopulation != null) return resultOfPopulation;

            resultOfPopulation = TryToPopulateRecurringToken(externalPaymentRequest);
            if (resultOfPopulation != null) return resultOfPopulation;

            var paymentResult = await _paymentService.SendPayment(externalPaymentRequest);

            var mappedPaymentTransaction = paymentResult.PaymentDto.ToEntity();
            mappedPaymentTransaction.SequenceNumber = sequenceNumber;
            mappedPaymentTransaction.MerchantSettings = paymentDto.MerchantSettings;
            mappedPaymentTransaction.Role = paymentDto.PaymentMeansReference.Role;

            paymentResult.PaymentDto.ExternalTransactionId = mappedPaymentTransaction.Id.ToString();

            _paymentTransactionService.Create(mappedPaymentTransaction);

            TransformAndUpdateRecurringToken(paymentResult.RecurringToken);

            return paymentResult.PaymentDto;
        }

        public Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto)
        {
            var preauthResult = await _paymentService.SendPreauth(dto);
            
            preauthResult.RecurringToken = TransformAndUpdateRecurringToken(preauthResult.RecurringToken);

            var preauthTransaction = preauthResult.ToEntity();

            preauthTransaction.SequenceNumber = 0;
            preauthTransaction.MerchantSettings = dto.MerchantSettings;
            preauthTransaction.Role = dto.PaymentMeansReference.Role;

            preauthResult.ExternalTransactionId = preauthTransaction.Id.ToString();

            _paymentTransactionService.Create(preauthTransaction);

            return preauthResult;
        }

        public Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId)
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

        public Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId)
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

        public Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId)
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

        public async Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId)
        {
            var paymentTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(transactionId);
            if (paymentTransaction != null && paymentTransaction is PreauthTransaction preauthTransaction)
            {
                return await _paymentService.SendCancellation(preauthTransaction.ToRequestDto());
            }

            return new ExternalPaymentCancellationDTO
            {
                Error = new ExternalIntegrationErrorDTO
                {
                    ErrorMessage = $"The transaction with id {transactionId} hasn't been found."
                }
            };
        }

        public async Task<ObjectResult> HandleWebhookAsync(string requestString)
        {
            var result = requestString.Replace("\n", string.Empty);
            try
            {
                var ts = new TransactionStatus(result);
            
                _logger.LogDebug($"PayOne: provider transaction id {ts.TxId}, status {ts.TxAction}");
            
                var pspTransaction = _paymentTransactionService.SinglePspTransactionByProviderTransactionId(ts.TxId);
            
                PaymentTransactionBase paymentTransaction;
                if (pspTransaction == null)
                {
                    paymentTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(ts.Param);
            
                    if (paymentTransaction == null)
                    {
                        _logger.LogWarning($"PayOne: provider transaction not found (id={ts.TxId},our reference={ts.Param}). Probably it is a transaction of a different system");
                        
                        return new OkObjectResult(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
                    }
            
                    if (paymentTransaction.GetType() == typeof(PreauthTransaction))
                    {
                        var captureTransaction = _paymentTransactionService.SingleByPreauthTransactionId((paymentTransaction as PreauthTransaction).GetId());
                        if (captureTransaction != null) 
                            paymentTransaction = captureTransaction;
                    }
                }
                else
                {
                    var referencedTransaction = pspTransaction.GetByTransactionId(ts.Param);
                    paymentTransaction = pspTransaction.GetLatest();
            
                    if (referencedTransaction == null || paymentTransaction == null)
                    {
                        _logger.LogWarning($"PayOne: Webhook for transaction {ts.Param} is invalid. Rejecting request");
            
                        return new BadRequestObjectResult(string.Empty);
                    }
                }

                MapPaymentTransactionStatus(ts, paymentTransaction, out RefundTransaction refund);
            
                if (int.TryParse(ts.Sequencenumber, out var sequenceNumber) && sequenceNumber > 0)
                {
                    if (sequenceNumber > paymentTransaction.SequenceNumber)
                    {
                        var wasUpdated = _paymentTransactionService.UpdateTransactionSeqNumber(paymentTransaction, sequenceNumber);
                        if (!wasUpdated)
                        {
                            _logger.LogError($"Updating SequenceNumber to {sequenceNumber} for transaction {paymentTransaction.Id} failed");
                        }
                    }
            
                    if (status != null)
                    {
                        var successOperations = GetTransactionSuccessOperationsCount(entityContext, paymentTransaction);
                        if (sequenceNumber + 1 < successOperations)
                        {
                            _logger.LogDebug($"Skip webhook for transactionId={paymentTransaction.Id} because it's old");
                        }
                    }
                }

                return new OkObjectResult(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process PayOne webhook: {result}", ex);
                
                return Error("Could not process event", HttpStatusCode.InternalServerError);
            }
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

        private ExternalPaymentTransactionDTO TryToPopulateRecurringToken(ExternalPaymentRequest externalPaymentRequest)
        {
            var externalRecurringToken = externalPaymentRequest.PaymentRequestDto.PaymentMeansReference.RecurringToken;
            if (string.IsNullOrWhiteSpace(externalRecurringToken))
                return null;

            var recurringToken = _recurringTokenService.SingleByIdOrDefault(
                ObjectId.Parse(externalRecurringToken));
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

            externalPaymentRequest.PaymentRequestDto.PaymentMeansReference.RecurringToken =
                _recurringTokenEncoder.Encrypt(recurringToken);

            return null;
        }

        private ExternalPaymentTransactionDTO TryToPopulatePreauthRequestDto(ExternalPaymentRequest externalPaymentRequest,
            out int sequenceNumber)
        {
            sequenceNumber = 0;

            var externalPreauthTransactionId = externalPaymentRequest?.PaymentRequestDto?.PaymentMeansReference?.PreauthTransactionId;
            if (string.IsNullOrWhiteSpace(externalPreauthTransactionId))
                return null;

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
        
        private decimal GetChargebackFeeAmount(TransactionStatus status, PaymentTransactionBase transaction)
        {
            var receivable = decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
            var fee = receivable - (transaction.RequestedAmount - transaction.RefundedAmount);
        
            if ((status.TxAction == "cancelation" || status.TxAction == "debit") && fee > 0m)
            {
                return fee;
            }

            return 0;
        }
        
        public void MapPaymentTransactionStatus(TransactionStatus status, PaymentTransactionBase transaction, out RefundTransaction refund)
        {
            decimal receivable = 0;
            decimal balance = 0;
            refund = null;

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
            switch (status.TxAction)
            {
                case "appointed":
                case "capture":
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
                        var targetBalance = paymentTransaction.RequestedAmount + chargebacksFee + chargebacksAmount - paidAmount;
 
                        RegisterPayment(status.Sequencenumber, paymentTransaction, targetBalance - balance);
                    }

                    break;
                case "refund":
                    if (paymentTransaction != null)
                    {
                        var chargebacksFee = paymentTransaction.Chargebacks?.Sum(c => c.FeeAmount) ?? 0;
                        var refundedAmount= (paymentTransaction.RequestedAmount + chargebacksFee - paymentTransaction.RefundedAmount) -
                                          receivable;

                        if (int.TryParse(status.Sequencenumber, out var sequenceNumber))
                        {
                            var refundTransactions = _paymentTransactionService.a
                            // Ignore webhook, because this status is already handled
                            if (transaction.StatusHistory.Any(s => s.RefundReference == sequenceNumber.ToString()))
                            {
                                return;
                            }

                            refund = new PaymentRefund
                            {
                                Amount = ptStatus.Amount.Value,
                                Status = PaymentRefundStatus.Succeeded
                            };
                        }
                    }

                    break;
                case "cancelation":
                    ptStatus.ProviderErrorCode = status.FailedCause;
                    ptStatus.RetryStrategy = PaymentRetryStrategy.NoRetry;
                    ptStatus.Status = PaymentStatusValue.Chargeback;
                    
                    if (string.IsNullOrWhiteSpace(status.FailedCause) == false)
                    {
                        var failedCause = status.FailedCause.ToLower();
                        
                        switch (failedCause)
                        {
                            case "soc":
                                ptStatus.ErrorCode = PaymentErrorCode.InsufficientBalance;
                                break;
                            case "cka":
                            case "uan":
                                ptStatus.ErrorCode = PaymentErrorCode.BearerInvalid;
                                break;
                            case "ndd":
                                ptStatus.ErrorCode = PaymentErrorCode.BearerInvalid;
                                break;
                            case "cb":
                            case "obj":
                                ptStatus.ErrorCode = PaymentErrorCode.Canceled;
                                break;
                            case "ret":
                            case "nelv":
                            case "ncc":
                                ptStatus.ErrorCode = PaymentErrorCode.Rejected;
                                break;
                            default:
                                ptStatus.ErrorCode = PaymentErrorCode.UnmappedError;
                                ptStatus.Status = PaymentStatusValue.Failed;
                                break;
                        }
                    }

                    break;
                default:
                    return;
            }
        }

        private static void RegisterPayment(string externalItemId, PaymentTransaction paymentTransaction, decimal amount)
        {
            paymentTransaction.Payments ??= new List<ExternalPaymentItemDTO>();
            var now = DateTime.Now;
            paymentTransaction.Payments.Add(new ExternalPaymentItemDTO
            {
                ExternalItemId = externalItemId,
                Amount = amount,
                BookingDate = new LocalDate(now.Year, now.Month, now.Day)
            });
        }

        #endregion private methods
    }
}
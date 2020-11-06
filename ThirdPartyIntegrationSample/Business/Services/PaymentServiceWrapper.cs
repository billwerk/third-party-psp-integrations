using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Billwerk.Payment.PayOne.Models;
using Billwerk.Payment.PayOne.Services;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Billwerk.Payment.SDK.Enums.ExternalIntegration;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Interfaces.Models;
using Business.Enums;
using Business.Helpers;
using Business.Interfaces;
using MongoDB.Bson;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Business.Services
{
    public class PaymentServiceWrapper : IPaymentServiceWrapper
    {
        private const string NotFoundErrorMessage = "Not Found";
        private const string InvalidPreconditionsErrorMessage = "Transaction Id is empty";

        private readonly PayOnePspService _payOnePaymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IRecurringTokenService _recurringTokenService;
        private readonly ILogger<PaymentServiceWrapper> _logger;

        public PaymentServiceWrapper(PayOnePspService payOnePaymentService, IPaymentTransactionService paymentTransactionService,
            IRecurringTokenService recurringTokenService, ILogger<PaymentServiceWrapper> logger)
        {
            _payOnePaymentService = payOnePaymentService;
            _paymentTransactionService = paymentTransactionService;
            _recurringTokenService = recurringTokenService;
            _logger = logger;
        }

        private IPspPaymentService GetPaymentService(PaymentServiceProvider provider)
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
            int sequenceNumber;
            ExternalIntegrationErrorDTO error;
            if (GetPreauthInfoByRequest(paymentDto, provider, out var preauthInfo, out sequenceNumber, out error))
            {
                return new ExternalPaymentTransactionDTO
                {
                    Error = error
                };
            }

            if (!GetRecurringTokenByRequest(paymentDto, out var recurringToken, out error))
            {
                return new ExternalPaymentTransactionDTO
                {
                    Error = error
                };
            }

            var paymentResult = await GetPaymentService(provider).SendPayment(paymentDto, recurringToken, preauthInfo);

            var mappedPaymentTransaction = paymentResult.PaymentDto.ToEntity();
            mappedPaymentTransaction.SequenceNumber = sequenceNumber;
            mappedPaymentTransaction.MerchantSettings = (ExternalIntegrationMerchantPspSettings)paymentDto.MerchantPspSettings;
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

            var paymentInfo = GetPaymentInfo(provider, targetTransaction);
            var refundResult = await GetPaymentService(provider).SendRefund(dto, paymentInfo);

            if (refundResult.Status == PaymentTransactionNewStatus.Succeeded)
            {
                _paymentTransactionService.UpdateTransactionSeqNumber(targetTransaction, targetTransaction.SequenceNumber + 1);
            }

            var refundTransaction = refundResult.ToEntity();

            refundTransaction.PaymentTransactionId = targetTransaction.Id.AsTyped<PaymentTransaction>();
            refundTransaction.MerchantSettings = (ExternalIntegrationMerchantPspSettings)dto.MerchantPspSettings;
            refundTransaction.Role = targetTransaction.Role;
            refundTransaction.WebhookTarget = dto.WebhookTarget;
            refundTransaction.SequenceNumber = targetTransaction.SequenceNumber;

            refundResult.ExternalTransactionId = refundTransaction.Id.ToString();

            _paymentTransactionService.Create(refundTransaction);

            return refundResult;
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(PaymentServiceProvider provider, ExternalPreauthRequestDTO dto)
        {
            IRecurringToken recurringToken = null;
            if (string.IsNullOrWhiteSpace(dto.PaymentMeansReference.InitialToken))
            {
                if (!GetRecurringTokenByRequest(dto, out recurringToken, out var error))
                {
                    return new ExternalPreauthTransactionDTO
                    {
                        Error = error
                    };
                }
            }

            var preauthResult = await GetPaymentService(provider).SendPreauth(dto, recurringToken);

            var preauthDto=preauthResult.PreauthDto;
            preauthDto.RecurringToken = TransformAndUpdateRecurringToken(preauthResult.RecurringToken);

            var preauthTransaction = preauthDto.ToEntity();

            preauthTransaction.SequenceNumber = 0;
            preauthTransaction.MerchantSettings = (ExternalIntegrationMerchantPspSettings)dto.MerchantPspSettings;
            preauthTransaction.Role = dto.PaymentMeansReference.Role;
            preauthTransaction.WebhookTarget = dto.WebhookTarget;

            preauthDto.ExternalTransactionId = preauthTransaction.Id.ToString();

            _paymentTransactionService.Create(preauthTransaction);

            return preauthDto;
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
                var preauthInfo = GetPreauthInfo(provider, preauthTransaction);
                return await GetPaymentService(provider).SendCancellation(preauthTransaction.ToRequestDto(), preauthInfo);
            }

            return new ExternalPaymentCancellationDTO
            {
                Error = new ExternalIntegrationErrorDTO
                {
                    ErrorMessage = $"The transaction with id {transactionId} hasn't been found."
                }
            };
        }

        private IPreauthInfo GetPreauthInfo(PaymentServiceProvider provider, PreauthTransaction preauthTransaction)
        {
            if (preauthTransaction!=null)
            {
                if (provider == PaymentServiceProvider.PayOne)
                {
                    return new PayOnePreauthInfo
                    {
                        Role = preauthTransaction.Role,
                        BearerDto = preauthTransaction.Bearer,
                        PspTransactionId = preauthTransaction.PspTransactionId
                    };
                }
                throw new NotSupportedException($"Provider={provider} is not supported!");
            }

            return null;
        }
        
        private IPaymentInfo GetPaymentInfo(PaymentServiceProvider provider, PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction!=null)
            {
                if (provider == PaymentServiceProvider.PayOne)
                {
                    return new PayOnePaymentInfo
                    {
                        ChargebacksAmount = paymentTransaction.Chargebacks?.Sum(c => c.Amount),
                        PaidAmount = paymentTransaction.Payments?.Sum(c => c.Amount),
                        ChargebacksFee = paymentTransaction.Chargebacks?.Sum(c => c.FeeAmount),
                        RefundedAmount = paymentTransaction.RefundedAmount,
                        RequestedAmount = paymentTransaction.RequestedAmount,
                        SequenceNumber = paymentTransaction.SequenceNumber,
                        InvoiceReferenceCode = paymentTransaction.InvoiceReferenceCode,
                        PspTransactionId = paymentTransaction.PspTransactionId
                    };
                }
                throw new NotSupportedException($"Provider={provider} is not supported!");
            }

            return null;
        }
        
        private IRefundInfo GetRefundInfo(PaymentServiceProvider provider, RefundTransaction refundTransaction)
        {
            if (refundTransaction != null)
            {
                if (provider == PaymentServiceProvider.PayOne)
                {
                    var refund = new PayOneRefundInfo();
                    if (refundTransaction.Refunds != null)
                    {
                        refund.RefundReferences = refundTransaction.Refunds.Select(r => r.ExternalItemId).ToList();
                    }

                    return refund;

                }
                throw new NotSupportedException($"Provider={provider} is not supported!");
            }

            return null;
        }

        public ObjectResult HandleWebhookAsync(PaymentServiceProvider provider, string requestString)
        {
            //return await GetPaymentService(provider).SendCancellation(preauthTransaction.ToRequestDto(), preauthTransaction);
            var result = requestString.Replace("\n", string.Empty);
            try
            {
                var service = GetPaymentService(provider);
                var data = service.GetDataFromWebhook(result);
                _logger.LogDebug($"{provider}: provider transaction id {data.PspTransactionId}");

                if (TryToIdentifyTransaction(provider, data, out var transaction, out var buildAcceptResult))
                {
                    return buildAcceptResult;
                }

                IPaymentInfo paymentInfo = null;
                IRefundInfo refundInfo = null;
                PaymentTransaction paymentTransaction = null;
                if (transaction is PaymentTransaction payment)
                {
                    paymentInfo = GetPaymentInfo(provider, payment);                    
                }
                else if (transaction is RefundTransaction refund)
                {
                    refundInfo = GetRefundInfo(provider, refund);
                    paymentTransaction = _paymentTransactionService.SingleByIdOrDefault(refund.PaymentTransactionId.Untyped) as PaymentTransaction;
                    paymentInfo = GetPaymentInfo(provider, paymentTransaction);
                }
                else
                {
                    throw new NotImplementedException($"Transaction type={transaction.GetType()} handling is not implemented!");
                }

                service.HandleWebhook(data, paymentInfo,  refundInfo, out var newPayment, out var newRefund, out var newChargeback, out var wasSkipped);
                RegisterPayment(transaction, newPayment);
                RegisterRefund(transaction, newRefund, paymentTransaction);
                RegisterChargeback(transaction, newChargeback);
                if (AnalyzeSequenceNumber(data.SequenceNumber, transaction, wasSkipped, out var objectResult))
                {
                    return objectResult;
                }

                _paymentTransactionService.Update(transaction);

                BackgroundJob.Enqueue<IWebhookService>(service => service.Send(transaction.WebhookTarget,PaymentServiceProvider.PayOne, transaction.ExternalTransactionId));

                return BuildAcceptResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process {provider} webhook: {result}", ex);

                return new InternalServerErrorMessageResult("Could not process event");
            }
        }

        private bool AnalyzeSequenceNumber(int sequenceNumber, Transaction transaction, bool wasSkipped,
            out ObjectResult objectResult)
        {
            var wasUpdated = false;
            objectResult = null;

            if (sequenceNumber > transaction.SequenceNumber)
            {
                wasUpdated = _paymentTransactionService.UpdateTransactionSeqNumber(transaction, sequenceNumber);
                if (!wasUpdated)
                {
                    _logger.LogError($"Updating SequenceNumber to {sequenceNumber} for transaction {transaction.Id} failed");
                }
            }

            if (!wasSkipped)
            {
                var successOperations = transaction.SequenceNumber;
                if (sequenceNumber + 1 < successOperations)
                {
                    _logger.LogDebug($"Skip webhook for transactionId={transaction.Id} because it's old");

                    {
                        objectResult = BuildAcceptResult();
                        return true;
                    }
                }
            }
            else if (!wasUpdated)
            {
                _logger.LogDebug($"Skip webhook for transactionId={transaction.Id} because of irrelevant webhook action and irrelevant sequence number");
                {
                    objectResult = BuildAcceptResult();
                    return true;
                }
            }

            return false;
        }

        private bool TryToIdentifyTransaction(PaymentServiceProvider provider, IPspWebhookData data, out Transaction transaction,
            out ObjectResult buildAcceptResult)
        {
            buildAcceptResult = null;

            var pspTransaction = _paymentTransactionService.SinglePspTransactionByProviderTransactionId(data.PspTransactionId);
            if (pspTransaction == null)
            {
                transaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(data.TransactionId);

                if (transaction == null)
                {
                    _logger.LogWarning($"{provider}: provider transaction not found (id={data.PspTransactionId},our reference={data.TransactionId}). Probably it is a transaction of a different system");
                    {
                        buildAcceptResult = BuildAcceptResult();
                        return true;
                    }
                }

                if (transaction.GetType() == typeof(PreauthTransaction))
                {
                    var captureTransaction = _paymentTransactionService.SingleByPreauthTransactionId((transaction as PreauthTransaction).GetId());
                    if (captureTransaction != null) transaction = captureTransaction;
                }
            }
            else
            {
                var referencedTransaction = pspTransaction.GetByExternalTransactionId(data.TransactionId);
                transaction = data.IsRefundAction
                    ? pspTransaction.GetRefundTransaction(data.RefundReference)
                    : pspTransaction.GetLatest();

                //Todo: if Action is refund and transaction == null => External Refund

                if (referencedTransaction == null || transaction == null)
                {
                    _logger.LogWarning($"{provider}: Webhook for transaction {data.TransactionId} is invalid. Rejecting request");
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

        private bool GetRecurringTokenByRequest(ExternalPaymentTransactionBasePaymentRequestDTO requestDto, out IRecurringToken recurringToken, out ExternalIntegrationErrorDTO error)
        {
            //TODO reimplement throwing exceptions
            var externalRecurringToken = requestDto.PaymentMeansReference.RecurringToken;
            error = null;
            recurringToken = null;
            if (string.IsNullOrWhiteSpace(externalRecurringToken))
            {
                error = new ExternalIntegrationErrorDTO
                {
                    ErrorCode = PaymentErrorCode.InvalidPreconditions,
                    ErrorMessage = $"Token is empty"
                };
                return false;
            }

            recurringToken = _recurringTokenService.SingleByIdOrDefault(ObjectId.Parse(externalRecurringToken));
            if (recurringToken == null)
            {
                error = new ExternalIntegrationErrorDTO
                {
                    ErrorCode = PaymentErrorCode.InvalidPreconditions,
                    ErrorMessage = $"Unknown recurringToken {externalRecurringToken}"
                };
                return false;
            }
            
            return true;
        }

        private bool GetPreauthInfoByRequest(ExternalPaymentRequestDTO externalPaymentRequest, PaymentServiceProvider provider,
            out IPreauthInfo preauthInfo, out int sequenceNumber, out ExternalIntegrationErrorDTO error)
        {
            sequenceNumber = 0;
            error = null;
            preauthInfo = null;

            var externalPreauthTransactionId = externalPaymentRequest.PaymentMeansReference?.PreauthTransactionId;
            if (string.IsNullOrWhiteSpace(externalPreauthTransactionId))
            {
                return true;
            }

            var paymentTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(externalPreauthTransactionId);

            if (paymentTransaction == null || !(paymentTransaction is PreauthTransaction preauthTransaction))
            {
                error = new ExternalIntegrationErrorDTO
                {
                    ErrorCode = PaymentErrorCode.InvalidPreconditions,
                    ErrorMessage = $"Unknown preauthTransactionId {externalPreauthTransactionId}"
                };
                return false;
            }

            if (provider == PaymentServiceProvider.PayOne)
            {
                preauthInfo=new PayOnePreauthInfo
                {
                    Role = preauthTransaction.Role,
                    BearerDto = preauthTransaction.Bearer,
                    PspTransactionId = preauthTransaction.PspTransactionId
                };
            }
            else
            {
                throw new NotSupportedException($"provider={provider} is not supported!");
            }

            sequenceNumber = 1;

            return true;
        }

        private string TransformAndUpdateRecurringToken(IRecurringToken token)
        {
            if (token == null)
            {
                return null;
            }

            var recurringToken = token.ToRecurringToken();
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

        private void RegisterRefund(Transaction transaction, ExternalRefundItemDTO newRefund, PaymentTransaction paymentTransaction)
        {
            if (transaction is RefundTransaction refundTransaction)
            {
                refundTransaction.Refunds ??= new List<ExternalRefundItemDTO>();
                refundTransaction.Refunds.Add(newRefund);
                paymentTransaction.RefundedAmount += refundTransaction.Refunds.Sum(r => r.Amount);
                //TODO implement per property update
                _paymentTransactionService.Update(paymentTransaction);
            }
        }

        private static void RegisterPayment(Transaction transaction, ExternalPaymentItemDTO newPayment)
        {
            if (transaction is PaymentTransaction paymentTransaction)
            {
                paymentTransaction.Payments ??= new List<ExternalPaymentItemDTO>();
                paymentTransaction.Payments.Add(newPayment);
            }
        }

        private static void RegisterChargeback(Transaction transaction, ExternalPaymentChargebackItemDTO newChargeback)
        {
            if (transaction is PaymentTransaction paymentTransaction)
            {
                paymentTransaction.Chargebacks ??= new List<ExternalPaymentChargebackItemDTO>();
                if (newChargeback.Reason == ExternalPaymentChargebackReason.Unknown)
                {
                    paymentTransaction.StatusHistory.Add(PaymentTransactionNewStatus.Failed);
                }

                paymentTransaction.Chargebacks.Add(newChargeback);
            }
        }

        #endregion private methods
    }
}
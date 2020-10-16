using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums.ExternalIntegration;
using Billwerk.Payment.SDK.Enums;
using Business.Helpers;
using Business.Interfaces;
using Business.PayOne.Model;
using MongoDB.Bson;
using NodaTime;
using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;
using Persistence.Models;

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
            throw new System.NotImplementedException();
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto)
        {
            var preauthResult = await _paymentService.SendPreauth(dto);

            var preauthTransaction = preauthResult.ToEntity();

            preauthTransaction.SequenceNumber = 0;
            preauthTransaction.MerchantSettings = dto.MerchantSettings;
            preauthTransaction.Role = dto.PaymentMeansReference.Role;

            preauthResult.ExternalTransactionId = preauthTransaction.Id.ToString();

            preauthResult.RecurringToken = TransformAndUpdateRecurringToken(preauthResult.RecurringToken);

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
            // var result = requestString.Replace("\n", string.Empty);
            // try
            // {
            //     var ts = new TransactionStatus(result);
            //
            //     _logger.LogDebug($"PayOne: provider transaction id {ts.TxId}, status {ts.TxAction}");
            //
            //     var pspTransaction = _paymentTransactionService.SinglePspTransactionByProviderTransactionId(ts.TxId);
            //
            //     PaymentTransaction paymentTransaction;
            //     HttpResponseMessage response;
            //
            //     if (pspTransaction == null)
            //     {
            //         paymentTransaction = _paymentTransactionService.SingleByIdOrDefaultUnauthorized(ts.Param);
            //
            //         if (paymentTransaction == null)
            //         {
            //             _logger.LogWarning($"PayOne: provider transaction not found (id={ts.TxId},our reference={ts.Param}). Probably it is a transaction of a different system");
            //
            //             // Make sure we don't get this webhook again
            //             return new OkObjectResult(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
            //         }
            //
            //         if (paymentTransaction.GetType() == typeof(PreauthTransaction))
            //         {
            //             var captureTransaction = _paymentTransactionService.SingleByPreauthTransactionId(ctx, paymentTransaction.Id);
            //             if (captureTransaction != null) 
            //                 paymentTransaction = captureTransaction;
            //         }
            //     }
            //     else
            //     {
            //         var referencedTransaction = pspTransaction.GetByTransactionId(ts.Param);
            //         paymentTransaction = pspTransaction.GetLatest();
            //
            //         if (referencedTransaction == null || paymentTransaction == null)
            //         {
            //             _logger.LogWarning($"PayOne: Webhook for transaction {ts.Param} is invalid. Rejecting request");
            //
            //             return new BadRequestObjectResult(string.Empty);
            //         }
            //     }
            //
            //     // Check chargeback fees
            //     var chargebackFee = _payoneService.CheckForChargebackFee(ts, paymentTransaction);
            //     if (chargebackFee != null)
            //     {
            //         chargebackFee = _chargebackFeeService.Create(entityContext, chargebackFee);
            //         _logger.LogDebug($"PayOne: Registered handling fee for transaction {paymentTransaction}");
            //     }
            //
            //     _payoneService.MapPaymentTransactionStatus(ts, paymentTransaction, chargebackFee, out var status,
            //         out var refund);
            //
            //     if (int.TryParse(ts.Sequencenumber, out var sequenceNumber) && sequenceNumber > 0)
            //     {
            //         if (sequenceNumber > paymentTransaction.SequenceNumber)
            //         {
            //             _paymentTransactionService.UpdatePayOneTransactionSeqNumber(entityContext, sequenceNumber,
            //                 paymentTransaction);
            //         }
            //
            //         if (status != null)
            //         {
            //             var successOperations = GetTransactionSuccessOperationsCount(entityContext, paymentTransaction);
            //
            //             if (sequenceNumber + 1 < successOperations)
            //             {
            //                 _logger.LogDebug($"Skip webhook for transactionId={paymentTransaction.Id} because it's old");
            //
            //                 status = null;
            //             }
            //         }
            //     }
            //
            //     if (status != null)
            //     {
            //         await HandlePSPWebhookAsync(entityContext, status, paymentTransaction, refund);
            //         
            //         _logger.LogDebug($"PayOne: webhook handled for payment transaction {paymentTransaction.Id}");
            //     }
            //
            //     return new OkObjectResult(new ByteArrayContent(new UTF8Encoding().GetBytes("TSOK")));
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError($"Failed to process PayOne webhook: {result}", ex);
            //     
            //     return Error("Could not process event", HttpStatusCode.InternalServerError);
            // }
            
            return new AcceptedResult();
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

        private void CheckForChargebackFee(TransactionStatus status, PaymentTransaction transaction)
        {
            var recivable = decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
            var fee = recivable - (transaction.RequestedAmount - transaction.RefundedAmount); //Todo HandlingFees

            if ((status.TxAction == "cancelation" || status.TxAction == "debit") && fee > 0m)
            {
                //Todo billChargebackFeeAutomatically

                var externalPaymentChargebackItem = new ExternalPaymentChargebackItemDTO
                {
                    ExternalItemId = "", //Todo timeStamp, Check webhook
                    Amount = transaction.RequestedAmount, //Todo check for partial chargebacks
                    BookingDate = new LocalDate(), //Todo ask Christian, check webhook
                    Description = "", //Todo check webhook
                    FeeAmount = fee,
                    Reason = ExternalPaymentChargebackReason.Unknown, //Todo
                    PspReason = "Reason" //Todo webhook
                };

                if (transaction.Chargebacks == null)
                {
                    transaction.Chargebacks = new List<ExternalPaymentChargebackItemDTO>
                    {
                        externalPaymentChargebackItem
                    };
                }
                else
                {
                    transaction.Chargebacks.Add(externalPaymentChargebackItem);
                }
            }
        }

        private void MapPaymentTransactionStatus(TransactionStatus status, PaymentTransactionBase transaction) //Todo recheck
        {
            decimal receivable = 0;
            decimal balance = 0;

            if (!string.IsNullOrWhiteSpace(status.Receivable))
            {
                receivable = decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrWhiteSpace(status.Balance))
            {
                balance = decimal.Parse(status.Balance, CultureInfo.InvariantCulture);
            }

            switch (status.TxAction)
            {
                case "appointed":
                case "capture":
                    // This status should not be relevant for any transaction. Initial Succeeded or PreliminarySucceeded
                    // is set during actual payment request already so this is redundant
                    return;
                case "paid":
                case "underpaid":
                    //Todo
                    decimal targetBalance = 0; //Todo

                    //Statuses

                    break;
                case "refund":
                    //refund

                    break;
                case "cancelation":
                    // billauto

                    if (string.IsNullOrWhiteSpace(status.FailedCause) == false)
                    {
                        var failedCause = status.FailedCause.ToLower();

                        switch (failedCause)
                        {
                            //error codes
                            case "soc":
                                break;
                            case "cka":
                            case "uan":
                                break;
                            case "ndd":
                                break;
                            case "cb":
                            case "obj":
                                break;
                            case "ret":
                            case "nelv":
                            case "ncc":
                                break;
                            default:
                                break;
                        }
                    }

                    break;
                default:
                    //Don't handle other webhooks
                    return;
            }

            //additional
        }

        #endregion private methods
    }
}
using System;
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
using Persistence.Interfaces;
using Persistence.Models;

namespace Business.Services
{
    public class PaymentServiceWrapper : IPaymentServiceWrapper
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IRecurringTokenService _recurringTokenService;
        private readonly IRecurringTokenEncoder<RecurringToken> _recurringTokenEncoder;
        private const string NotFoundErrorMessage = "Not Found";
        private const string InvalidPreconditionsErrorMessage = "Transaction Id is empty";

        public PaymentServiceWrapper(IPaymentService paymentService, IPaymentTransactionService paymentTransactionService,
            IRecurringTokenService recurringTokenService, IRecurringTokenEncoder<RecurringToken> recurringTokenEncoder)
        {
            _paymentService = paymentService;
            _paymentTransactionService = paymentTransactionService;
            _recurringTokenService = recurringTokenService;
            _recurringTokenEncoder = recurringTokenEncoder;
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

        public async Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto)
        {

            var targetTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(dto.TransactionId);

            if (targetTransaction == null)
            {
                return new ExternalRefundTransactionDTO
                {
                    Error = CreateUnmappedError()
                };
            }

            var refundResult = await _paymentService.SendRefund(dto, targetTransaction);

            var refundTransaction = refundResult.ToEntity();

            //Todo is it necessary here?
            refundTransaction.SequenceNumber = targetTransaction.SequenceNumber + 1;

            refundTransaction.MerchantSettings = dto.MerchantSettings;
            refundTransaction.Role = targetTransaction.Role;

            refundResult.ExternalTransactionId = refundTransaction.Id.ToString();

            _paymentTransactionService.Create(refundTransaction);

            return refundResult;
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
            if (String.IsNullOrEmpty(transactionId))
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
            if (String.IsNullOrEmpty(transactionId))
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
            if (String.IsNullOrEmpty(transactionId))
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

        #region Private methods

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
            if (string.IsNullOrWhiteSpace(externalPaymentRequest.PaymentRequestDto.PaymentMeansReference.RecurringToken))
                return null;

            var recurringToken = _recurringTokenService.SingleByIdOrDefault(
                ObjectId.Parse(externalPaymentRequest.PaymentRequestDto.PaymentMeansReference.RecurringToken));
            if (recurringToken == null)
            {
                return new ExternalPaymentTransactionDTO
                {
                    Error = new ExternalIntegrationErrorDTO
                    {
                        ErrorCode = PaymentErrorCode.InvalidPreconditions,
                        ErrorMessage = "Unknown recurringToken"
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

            if (string.IsNullOrWhiteSpace(externalPaymentRequest.PaymentRequestDto.PaymentMeansReference.PreauthTransactionId))
                return null;

            var paymentTransaction =
                _paymentTransactionService.SingleByExternalTransactionIdOrDefault(externalPaymentRequest.PaymentRequestDto
                    .PaymentMeansReference.PreauthTransactionId);

            if (paymentTransaction == null || !(paymentTransaction is PreauthTransaction preauthTransaction))
                return new ExternalPaymentTransactionDTO
                {
                    Error = new ExternalIntegrationErrorDTO
                    {
                        ErrorCode = PaymentErrorCode.InvalidPreconditions,
                        ErrorMessage = "Unknown recurringToken"
                    }
                };

            externalPaymentRequest.PreauthRequestDto = preauthTransaction.ToRequestDto();
            externalPaymentRequest.BearerDto = preauthTransaction.Bearer;
            externalPaymentRequest.PspTransactionId = preauthTransaction.PspTransactionId;

            sequenceNumber = 1;

            return null;
        }

        #endregion

        public Task<string> HandleWebhookAsync(string requestString)
        {
            var result = requestString.Replace("\n", string.Empty);
            try
            {
                var ts = new TransactionStatus(result);
                var pspTransaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(ts.TxId); //Todo Double
                PaymentTransactionBase transaction;
                if (pspTransaction == null)
                {
                    transaction = _paymentTransactionService.SingleByExternalTransactionIdOrDefault(ts.Param);

                    if (transaction == null)
                    {
                        return null; //Todo
                    }

                    if (transaction.GetType() == typeof(PreauthTransaction))
                    {
                        var paymentTransaction = (PaymentTransaction)_paymentTransactionService.SingleByIdOrDefault(transaction.Id); //Todo SingleByPreauthTransactionId to find capture transaction by preauth
                        if (paymentTransaction != null)
                        {
                            transaction = paymentTransaction;
                        }
                    }
                }
                else
                {
                    var referencedTransaction = pspTransaction; //Todo referenced transaction by SingleHackTransaction
                    transaction = pspTransaction; //Todo latest transaction by SingleHackTransaction
                    if (referencedTransaction == null || transaction == null)
                    {
                        return null; //Todo
                    }
                }

                //Check chargeback fees
                if (transaction.GetType() == typeof(PaymentTransaction))
                {
                    CheckForChargebackFee(ts, (PaymentTransaction)transaction);
                }

                //Map? 
                MapPaymentTransactionStatus(ts, transaction);

                if (Int32.TryParse(ts.Sequencenumber, out var sequenceNumber) && sequenceNumber > 0)
                {
                    if (sequenceNumber > transaction.SequenceNumber)
                    {
                        transaction.SequenceNumber = sequenceNumber;
                    }
                    //Todo Billwerk?
                }

                _paymentTransactionService.Update(transaction);

                //if (status != null)
                //{
                //    //Send Webhook to Billwerk
                //}

                return Task.FromResult("");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void CheckForChargebackFee(TransactionStatus status, PaymentTransaction transaction)
        {
            var recivable = Decimal.Parse(status.Receivable, CultureInfo.InvariantCulture);
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
                    transaction.Chargebacks = new List<ExternalPaymentChargebackItemDTO> { externalPaymentChargebackItem };
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
    }
}
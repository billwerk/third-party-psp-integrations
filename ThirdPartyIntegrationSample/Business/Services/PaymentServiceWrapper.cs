using System;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Business.Helpers;
using Business.Interfaces;
using Business.Models;
using MongoDB.Bson;
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
                return await _paymentService.SendCancellation(preauthTransaction.ToDto());
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

            externalPaymentRequest.PreauthRequestDto = preauthTransaction.ToDto();
            externalPaymentRequest.BearerDto = preauthTransaction.Bearer;
            externalPaymentRequest.PspTransactionId = preauthTransaction.PspTransactionId;

            sequenceNumber = 1;

            return null;
        }
    }
}
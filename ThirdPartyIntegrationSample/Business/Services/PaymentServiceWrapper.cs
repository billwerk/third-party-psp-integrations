using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Helpers;
using Business.Interfaces;
using Business.Models;
using Persistence.Interfaces;
using Persistence.Models;

namespace Business.Services
{
    public class PaymentServiceWrapper : IPaymentServiceWrapper
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentServiceWrapper(IPaymentService paymentService, IPaymentTransactionService paymentTransactionService)
        {
            _paymentService = paymentService;
            _paymentTransactionService = paymentTransactionService;
        }

        public async Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO paymentDto)
        {
            var externalPaymentRequest = new ExternalPaymentRequest
            {
                PaymentRequestDto = paymentDto
            };
            
            if (!string.IsNullOrWhiteSpace(paymentDto.PaymentMeansReference.PreauthTransactionId))
            {
                var paymentTransaction =
                    _paymentTransactionService.SingleByExternalTransactionIdOrDefault(paymentDto.PaymentMeansReference
                        .PreauthTransactionId);

                if (paymentTransaction != null && paymentTransaction is PreauthTransaction preauthTransaction)
                {
                    externalPaymentRequest.PreauthRequestDto = preauthTransaction.ToDto();
                    externalPaymentRequest.BearerDto = preauthTransaction.Bearer;
                    externalPaymentRequest.PspTransactionId = preauthTransaction.PspTransactionId;
                }

                var paymentResult = await _paymentService.SendPayment(externalPaymentRequest);

                var mappedPaymentTransaction = paymentResult.ToEntity();
                mappedPaymentTransaction.SequenceNumber = 1;
                mappedPaymentTransaction.MerchantSettings = paymentDto.MerchantSettings;
                mappedPaymentTransaction.Role = paymentDto.PaymentMeansReference.Role;
                    
                paymentResult.ExternalTransactionId = mappedPaymentTransaction.Id.ToString();
                
                _paymentTransactionService.Create(mappedPaymentTransaction);

                return paymentResult;
            }

            return new ExternalPaymentTransactionDTO();
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

            _paymentTransactionService.Create(preauthTransaction);

            return preauthResult;
        }

        public Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId)
        {
            throw new System.NotImplementedException();
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

    }
}
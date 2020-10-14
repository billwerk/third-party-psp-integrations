using System.Threading.Tasks;
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

        public Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId)
        {
            throw new System.NotImplementedException();
        }
    }
}
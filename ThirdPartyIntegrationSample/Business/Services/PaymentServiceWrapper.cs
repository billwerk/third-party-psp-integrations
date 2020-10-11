using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Business.Interfaces;
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


        public Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto)
        {
            var preauthTransaction = new PreauthTransaction
            {
                AuthorizedAmount = dto.RequestedAmount,
                Currency = dto.Currency,
                ExternalTransactionId = dto.PaymentMeansReference.PreauthTransactionId
            };
            preauthTransaction.ForceId();
            dto.TransactionId = preauthTransaction.Id.ToString();
            preauthTransaction.StatusHistory.Add(PaymentTransactionNewStatus.Initiated);

            
            var preauthResult = await _paymentService.SendPreauth(dto);
            preauthResult.ExternalTransactionId = preauthTransaction.Id.ToString();
            
            preauthTransaction.StatusHistory.Add(preauthResult.Status);
            preauthTransaction.LastUpdated = preauthResult.LastUpdated;
            preauthTransaction.AuthorizedAmount = preauthResult.AuthorizedAmount;
            preauthTransaction.RequestedAmount = preauthResult.RequestedAmount;
            preauthTransaction.SequenceNumber = 0;
            preauthTransaction.Bearer = preauthResult.Bearer;
            preauthTransaction.ExternalTransactionId = dto.TransactionId;
            preauthTransaction.PspTransactionId = preauthResult.ExternalTransactionId;
            preauthTransaction.ExpiresAt = preauthResult.ExpiresAt;

            _paymentTransactionService.Create(preauthTransaction);

            return preauthResult;
        }

        public Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId)
        {
            throw new System.NotImplementedException();
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
    }
}
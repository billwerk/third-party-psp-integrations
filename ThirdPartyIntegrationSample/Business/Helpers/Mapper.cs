using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Persistence.Models;

namespace Business.Helpers
{
    public static class Mapper
    {
        public static PreauthTransaction ToEntity(this ExternalPreauthTransactionDTO dto)
        {
            var preauthTransaction = new PreauthTransaction
            {
                AuthorizedAmount = dto.RequestedAmount,
                Currency = dto.Currency,
                LastUpdated = dto.LastUpdated,
                RequestedAmount = dto.RequestedAmount,
                Bearer = dto.Bearer,
                ExternalTransactionId = dto.TransactionId,
                PspTransactionId = dto.PspTransactionId,
                ExpiresAt = dto.ExpiresAt
            };
            
            preauthTransaction.ForceId();

            preauthTransaction.StatusHistory.Add(dto.Status);

            return preauthTransaction;
        }
        
        public static PaymentTransaction ToEntity(this ExternalPaymentTransactionDTO dto)
        {
            var paymentTransaction = new PaymentTransaction
            {
                Currency = dto.Currency,
                LastUpdated = dto.LastUpdated,
                RequestedAmount = dto.RequestedAmount,
                Bearer = dto.Bearer,
                ExternalTransactionId = dto.TransactionId,
                PspTransactionId = dto.PspTransactionId,
                DueDate = dto.DueDate,
                RefundableAmount = dto.RefundableAmount,
                RefundedAmount = dto.RefundedAmount
            };
            
            paymentTransaction.ForceId();

            paymentTransaction.StatusHistory.Add(dto.Status);

            return paymentTransaction;
        }
        
        public static ExternalPreauthTransactionDTO ToDto(this PreauthTransaction entity)
        {
            var preauthTransaction = new ExternalPreauthTransactionDTO
            {
                AuthorizedAmount = entity.RequestedAmount,
                Currency = entity.Currency,
                LastUpdated = entity.LastUpdated,
                RequestedAmount = entity.RequestedAmount,
                Bearer = entity.Bearer,
                ExternalTransactionId = entity.Id.ToString(),
                PspTransactionId = entity.PspTransactionId,
                ExpiresAt = entity.ExpiresAt,
                Status = entity.Status.Value,
                TransactionId = entity.ExternalTransactionId
            };

            return preauthTransaction;
        }
    }
}
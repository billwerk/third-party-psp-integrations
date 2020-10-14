using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Persistence.Models;

namespace Business.Helpers
{
    public static class Mapper
    {
        public static PreauthTransaction ToEntity(this ExternalPreauthTransactionDTO dto)
        {
            var preauthTransaction = ToEntity<PreauthTransaction, ExternalPreauthTransactionDTO>(dto);
            
            preauthTransaction.AuthorizedAmount = dto.AuthorizedAmount;
            preauthTransaction.Bearer = dto.Bearer;
            preauthTransaction.ExpiresAt = dto.ExpiresAt;

            return preauthTransaction;
        }

        public static PaymentTransaction ToEntity(this ExternalPaymentTransactionDTO dto)
        {
            var paymentTransaction = ToEntity<PaymentTransaction, ExternalPaymentTransactionDTO>(dto);

            paymentTransaction.Bearer = dto.Bearer;
            paymentTransaction.DueDate = dto.DueDate;
            paymentTransaction.RefundableAmount = dto.RefundableAmount;
            paymentTransaction.RefundedAmount = dto.RefundedAmount;

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

        private static TEntity ToEntity<TEntity, TDto>(this TDto dto) 
            where TDto : ExternalPaymentTransactionBaseDTO
            where TEntity : PaymentTransactionBase, new()
        {
            var paymentTransaction = new TEntity
            {
                Currency = dto.Currency,
                LastUpdated = dto.LastUpdated,
                RequestedAmount = dto.RequestedAmount,
                ExternalTransactionId = dto.TransactionId,
                PspTransactionId = dto.PspTransactionId,
            };

            paymentTransaction.ForceId();
            paymentTransaction.StatusHistory.Add(dto.Status);

            return paymentTransaction;
        }
    }
}
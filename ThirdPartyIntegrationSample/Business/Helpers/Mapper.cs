using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
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
            preauthTransaction.RecurringToken = dto.RecurringToken;

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

        public static RefundTransaction ToEntity(this ExternalRefundTransactionDTO dto)
        {
            var refundTransaction = ToEntity<RefundTransaction, ExternalRefundTransactionDTO>(dto);

            refundTransaction.Refunds = dto.Refunds;

            return refundTransaction;
        }

        public static ExternalPreauthRequestDTO ToRequestDto(this PreauthTransaction entity)
        {
            var preauthTransaction = new ExternalPreauthRequestDTO
            {
                Currency = entity.Currency,
                RequestedAmount = entity.RequestedAmount,
                TransactionId = entity.ExternalTransactionId,
                MerchantSettings = entity.MerchantSettings,
                PaymentMeansReference = new ExternalPaymentMeansReferenceDTO
                {
                    Role = entity.Role
                }
            };

            return preauthTransaction;
        }

        public static ExternalPaymentTransactionDTO ToDto(this PaymentTransaction entity)
        {
            var paymentTransaction = ToDto<ExternalPaymentTransactionDTO, PaymentTransaction>(entity);

            paymentTransaction.Payments = entity.Payments;
            paymentTransaction.Chargebacks = entity.Chargebacks;
            paymentTransaction.RefundedAmount = entity.RefundedAmount;
            paymentTransaction.RefundableAmount = entity.RefundableAmount;
            paymentTransaction.Bearer = entity.Bearer;
            paymentTransaction.DueDate = entity.DueDate;

            return paymentTransaction;
        }

        public static ExternalPreauthTransactionDTO ToDto(this PreauthTransaction entity)
        {
            var preauthTransaction = ToDto<ExternalPreauthTransactionDTO, PreauthTransaction>(entity);

            preauthTransaction.AuthorizedAmount = entity.AuthorizedAmount;
            preauthTransaction.ExpiresAt = entity.ExpiresAt;
            preauthTransaction.Bearer = entity.Bearer;
            preauthTransaction.RecurringToken = entity.RecurringToken;

            return preauthTransaction;
        }

        public static ExternalRefundTransactionDTO ToDto(this RefundTransaction entity)
        {
            var refundTransaction = ToDto<ExternalRefundTransactionDTO, RefundTransaction>(entity);

            refundTransaction.Refunds = entity.Refunds;

            return refundTransaction;
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

        private static TDto ToDto<TDto, TEntity>(this TEntity entity) where TEntity : PaymentTransactionBase
            where TDto : ExternalPaymentTransactionBaseDTO, new()
        {
            var transaction = new TDto
            {
                TransactionId = entity.ExternalTransactionId,
                ExternalTransactionId = entity.Id.ToString(),
                PspTransactionId = entity.PspTransactionId,
                Currency = entity.Currency,
                RequestedAmount = entity.RequestedAmount,
                LastUpdated = entity.LastUpdated,
                Status = entity.Status.Value
            };

            return transaction;
        }
    }
}
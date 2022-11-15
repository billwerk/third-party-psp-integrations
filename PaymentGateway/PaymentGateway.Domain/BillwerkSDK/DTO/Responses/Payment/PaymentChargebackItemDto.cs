using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using NodaTime;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Responses.Payment
{
    /// <summary>
    /// Represents data about single money movement (chargeback) within a transaction.
    /// </summary>
    public class PaymentChargebackItemDto : IPaymentItemDto
    {
        /// <summary>
        /// The fee amount of chargeback.
        /// Optional.
        /// Positive number.
        /// </summary>
        public decimal FeeAmount { get; set; }
        
        /// <summary>
        /// The reason of chargeback.
        /// </summary>
        public PaymentChargebackReason Reason { get; set; }
        
        /// <summary>
        /// The reason of chargeback from PSP side.
        /// </summary>
        public string PspReasonCode { get; set; }
        
        /// <summary>
        /// Detailed chargeback reason description.
        /// </summary>
        public string PspReasonMessage { get; set; }
        
        /// <summary>
        /// The value taken from PSP side to identify a linked payment transaction.
        /// Optional.
        /// </summary>
        public string ExternalItemId { get; set; }
        
        /// <summary>
        /// Payment transaction amount.
        /// Mandatory.
        /// Positive number.
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// The date of a payment transaction on PSP side.
        /// Mandatory.
        /// </summary>
        public LocalDate BookingDate { get; set; }
        
        /// <summary>
        /// The description of a payment transaction.
        /// Optional.
        /// </summary>
        public string Description { get; set; }
    }
}

using NodaTime;

namespace Billwerk.Payment.SDK.DTO.Responses.Payment
{
    /// <summary>
    /// Represents data about single money movement (payment) within a transaction.
    /// </summary>
    public class PaymentItemDto : IPaymentItemDto
    {
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

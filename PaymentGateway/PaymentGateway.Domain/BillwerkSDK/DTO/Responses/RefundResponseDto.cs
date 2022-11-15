using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using Billwerk.Payment.SDK.Enums;
using NodaTime;

namespace Billwerk.Payment.SDK.DTO.Responses
{
    /// <summary>
    /// Represents response data of refund call from billwerk to Integrator.
    /// </summary>
    public class RefundResponseDto : ITransactionResponseDto
    {
        /// <summary>
        /// The complete refunded amount.
        /// Mandatory.
        /// Positive or equaled to zero number.
        /// </summary>
        public decimal RefundedAmount { get; set; }

        /// <summary>
        /// The date when refund will process on PSP side.
        /// Can be provided by PSP or can be set to the date time of PSP notification.
        /// Mandatory.
        /// </summary>
        public LocalDate BookingDate { get; set; }

        /// <summary>
        /// The description of refund provided by PSP.
        /// Optional.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Transaction identifier specified by the integrator.
        /// Mandatory.
        /// </summary>
        public string ExternalTransactionId { get; set; }

        /// <summary>
        /// Transaction identifier specified by the payment provider.
        /// Optional (finalize and initial preauth considers case of empty PspTransactionId).
        /// </summary>
        public string PspTransactionId { get; set; }
        
        /// <summary>
        /// The currency of transaction.
        /// ISO-4217 format.
        /// Mandatory.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The amount requested for the call.
        /// Positive number.
        /// Mandatory.
        /// </summary>
        public decimal RequestedAmount { get; set; }
        
        /// <summary>
        /// Date time of the last update. Used to prevent older versions overwriting by new ones.
        /// DateTime UTC format.
        /// Mandatory.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Payment transaction status.
        /// Mandatory.
        /// </summary>
        public PaymentTransactionStatus Status { get; set; }
        
        /// <summary>
        /// The identifier of billwerk payment transaction, used for consistency.
        /// Mandatory.
        /// </summary>
        public string TransactionId { get; set; }
    }
}

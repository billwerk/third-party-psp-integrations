using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using Billwerk.Payment.SDK.Enums;

namespace Billwerk.Payment.SDK.DTO.Responses
{
    /// <summary>
    /// Represents response data of preauth call from billwerk to Integrator.
    /// </summary>
    public class PreauthResponseDto : ITransactionResponseDto
    {
        /// <summary>
        /// Authorized amount.
        /// Mandatory.
        /// If the PSP has a minimum preauth amount, this can be larger than the requested amount.
        /// If the PSP doesn't support reserving money in a preauth, this should be 0.
        /// </summary>
        public decimal AuthorizedAmount { get; set; }

        /// <summary>
        /// The expiration time of preauth transaction.
        /// Mandatory.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// The bearer that represents customer settings payment data taken from PSP side.
        /// Mandatory.
        /// </summary>
        public IDictionary<string, string> Bearer { get; set; }

        /// <summary>
        /// The URL provided by PSP to redirect payer after preauth.
        /// Mandatory.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Redirect data provided by PSP to redirect payer after preauth.
        /// </summary>
        public IDictionary<string, string> RedirectData { get; set; }

        /// <summary>
        /// Transaction identifier specified by the integrator.
        /// Mandatory.
        /// </summary>
        public string ExternalTransactionId { get; set; }

        /// <summary>
        /// Transaction identifier specified by the payment provider.
        /// Mandatory.
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

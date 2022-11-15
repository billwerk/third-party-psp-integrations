using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;

namespace Billwerk.Payment.SDK.DTO.Responses.Interfaces
{
    /// <summary>
    /// Represents transaction related response data provided to billwerk by Integrator.
    /// </summary>
    public interface ITransactionResponseDto : IResponseDto
    {
        /// <summary>
        /// Transaction identifier specified by the integrator.
        /// Mandatory.
        /// </summary>
        string ExternalTransactionId { get; set; }

        /// <summary>
        /// Transaction identifier specified by the payment provider.
        /// Mandatory.
        /// </summary>
        string PspTransactionId { get; set; }
        
        /// <summary>
        /// The currency of transaction.
        /// ISO-4217 format.
        /// Mandatory.
        /// </summary>
        string Currency { get; set; }

        /// <summary>
        /// The amount requested for the call.
        /// Positive number.
        /// Mandatory.
        /// </summary>
        decimal RequestedAmount { get; set; }
        
        /// <summary>
        /// Date time of the last update. Used to prevent older versions overwriting by new ones.
        /// DateTime UTC format.
        /// Mandatory.
        /// </summary>
        DateTime LastUpdated { get; set; }

        /// <summary>
        /// Payment transaction status.
        /// Mandatory.
        /// </summary>
        PaymentTransactionStatus Status { get; set; }
        
        /// <summary>
        /// The identifier of billwerk payment transaction, used for consistency.
        /// Mandatory.
        /// </summary>
        string TransactionId { get; set; }
    }
}

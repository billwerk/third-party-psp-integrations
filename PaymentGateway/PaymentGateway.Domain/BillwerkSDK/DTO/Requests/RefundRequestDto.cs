using Billwerk.Payment.SDK.DTO.Requests.InvoiceData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;

namespace Billwerk.Payment.SDK.DTO.Requests
{
    /// <summary>
    /// Represent request for refund operation.
    /// </summary>
    public class RefundRequestDto : IRequestDto
    {
        /// <summary>
        /// The transaction identifier from billwerk side.
        /// Mandatory.
        /// <b>Not used now, since part of not ended integration flow with billwerk.</b>
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Refund requested amount.
        /// Mandatory.
        /// Positive number.
        /// </summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>
        /// The currency of preauth.
        /// ISO-4217 format.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The notification URL from billwerk side.
        /// Mandatory.
        /// </summary>
        public string WebhookTarget { get; set; }

        /// <summary>
        /// The identifier of PSP settings to identify a PSP module.
        /// Mandatory.
        /// </summary>
        public string PspSettingsId { get; set; }

        /// <summary>
        /// The identifier of a billwerk payment transaction to be applied by refund operation.
        /// Mandatory.
        /// </summary>
        public string PaymentTransactionId { get; set; }

        /// <summary>
        /// The payer data.
        /// Optional
        /// </summary>
        public PayerDataDto PayerData { get; set; }

        /// <summary>
        /// Refund related credit note data.
        /// Optional, value specified if PSP supports the invoice data.
        /// PSPSettings.HasSupportInvoiceData prop.
        /// </summary>
        public InvoiceDataDto InvoiceData { get; set; }
    }
}

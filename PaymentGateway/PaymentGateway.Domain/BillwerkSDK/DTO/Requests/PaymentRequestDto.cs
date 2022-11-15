using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Requests.InvoiceData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Requests.PaymentReferenceData;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Requests
{
    /// <summary>
    /// Represent recurring or capture request.
    /// For capture request, PreauthTransactionId must be specified.
    /// </summary>
    public class PaymentRequestDto : IRequestDto, IHasAgreementIdDto
    {
        /// <summary>
        /// The identifier of PSP settings to identify a PSP module.
        /// Mandatory.
        /// <b>Not used now, since part of not ended integration flow with billwerk.</b>
        /// </summary>
        public string PspSettingsId { get; set; }
        
        /// <summary>
        /// The transaction identifier from billwerk side.
        /// Mandatory.
        /// </summary>
        public string TransactionId { get; set; }
        
        /// <summary>
        /// The identifier of billwerk preauth transaction. Indicate, that this payment request init for capture.
        /// Optional.
        /// </summary>
        public string PreauthTransactionId { get; set; }

        /// <summary>
        /// Payment requested amount.
        /// Mandatory.
        /// Positive number.
        /// </summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>
        /// The currency of payment.
        /// Mandatory.
        /// ISO-4217 format.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The notification URL from Billwerk side.
        /// Mandatory.
        /// A string containing the entire absolute URL.
        /// </summary>
        public string WebhookTarget { get; set; }

        /// <summary>
        /// The payment means data.
        /// Mandatory.
        /// </summary>
        public PaymentMeansReferenceDto PaymentMeansReference { get; set; }
        
        /// <summary>
        /// The reference text of invoice linked to payment transaction.
        /// Optional, experimental one! Can be changed/replaced in future!
        /// Includes linked invoice reference code, description and unique reference of transaction.
        /// </summary>
        public string TransactionInvoiceReferenceText { get; set; }

        /// <summary>
        /// The reference text for transaction.
        /// Mandatory.
        /// Includes Billwerk payment transaction id, description and unique reference of transaction.
        /// </summary>
        public string TransactionReferenceText { get; set; }

        /// <summary>
        /// The payer data.
        /// Mandatory.
        /// </summary>
        public PayerDataDto PayerData { get; set; }

        /// <summary>
        /// Calculated Direct Debit planned execution date of a payment.
        /// It is based on prenotification days count configured in PSP settings, or its default value.
        /// Calculation excludes bank holidays.
        /// Mandatory for Direct Debit only.
        /// </summary>
        public LocalDate? PlannedExecutionDate { get; set; }

        /// <summary>
        /// The data of invoice linked to a payment.
        /// Optional, null for capture always.
        /// For recurring requests, value specified if PSP supports the invoice data.
        /// PSPSettings.HasSupportInvoiceData prop.
        /// </summary>
        public InvoiceDataDto InvoiceData { get; set; }

        /// <summary>
        /// Id which specify agreement between 3-rd party integration and billwerk according payment actions,
        /// which initiated by billwerk for specific customer and supported by 3-rd party integration.
        /// Mandatory.
        /// </summary>
        public string AgreementId { get; set; }
    }
}

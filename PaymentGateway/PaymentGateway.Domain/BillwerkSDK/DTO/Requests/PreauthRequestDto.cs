using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Requests.OrderData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Requests.PaymentReferenceData;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;

namespace PaymentGateway.Domain.BillwerkSDK.DTO.Requests
{
    /// <summary>
    /// Represent preauth request. Can be not initial transaction between customer and PSP.
    /// Also used for migration flow.
    /// </summary>
    public class PreauthRequestDto : IRequestDto, IHasAgreementIdDto
    {
        /// <summary>
        /// The identifier of PSP settings to identify a PSP module.
        /// Mandatory.
        /// <b>Not used now, since part of not ended integration flow with billwerk.</b>
        /// </summary>
        public string PspSettingsId { get; set; }
        
        /// <summary>
        /// The transaction identifier from billwerk side.
        /// Format: {yyyyMMdd}-{ObjectId}
        /// Mandatory.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Preauth requested amount.
        /// Mandatory.
        /// Not negative number.
        /// </summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>
        /// The currency of preauth.
        /// Mandatory.
        /// ISO-4217 format.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The notification URL from billwerk side.
        /// Mandatory.
        /// </summary>
        public string WebhookTarget { get; set; }

        /// <summary>
        /// The payment means data.
        /// Mandatory.
        /// </summary>
        public PaymentMeansReferenceDto PaymentMeansReference { get; set; }
        
        /// <summary>
        /// The reference text of invoice linked to preauth transaction.
        /// Optional, experimental one! Can be changed/replaced in future!
        /// Includes linked invoice reference code, description and unique reference of transaction.
        /// Mandatory.
        /// </summary>
        public string TransactionInvoiceReferenceText { get; set; }

        /// <summary>
        /// The reference text for transaction.
        /// Includes billwerk preauth transaction id, description and unique reference of transaction.
        /// Mandatory.
        /// </summary>
        public string TransactionReferenceText { get; set; }

        /// <summary>
        /// The payer data.
        /// Mandatory.
        /// </summary>
        public PayerDataDto PayerData { get; set; }
        
        /// <summary>
        /// The order data.
        /// Optional, only for PSP's which need order data for preauth action.
        /// PSPSettings.HasSupportOrderDataInPreauth prop.
        /// </summary>
        public OrderDataDto OrderData { get; set; }
        
        /// <summary>
        /// Id which specify agreement between 3-rd party integration and billwerk according payment actions,
        /// which initiated by billwerk for specific customer and supported by 3-rd party integration.
        /// Mandatory.
        /// </summary>
        public string AgreementId { get; set; }

        #region Fields for rework
        
        /// <summary>
        /// The data required for migration via technical bearer
        /// Used to identify existing agreement/customer on PSP side to migrate contract date to billwerk. Optional
        /// </summary>
        public string MigrationToken { get; set; }
        
        /// <summary>
        /// Indicate, if preauth transaction initial or not.
        /// Mandatory. WARNING - can be replaced with separate endpoints for initial and non-initial preauth calls.
        /// </summary>
        public bool IsInitial { get; set; }

        #endregion
    }
}

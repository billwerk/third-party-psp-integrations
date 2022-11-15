using Billwerk.Payment.SDK.DTO.IntegrationInfo;

namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo
{
    /// <summary>
    ///  Specify billwerk payment process behavior.
    /// </summary>
    public class PaymentDirectDebitMethodInfoDto : PaymentMethodInfoDto
    {
        /// <summary>
        /// Indicated that payment provider supports signup without direct interaction with customer.
        /// F.e. using IBAN.
        /// </summary>
        public bool SupportBackendPayments { get; set; }
        
        /// <summary>
        /// Indicates that Ledgers, created for Refunds should not be confirmed automatically.
        /// </summary>
        public bool UnconfirmedLedgerForRefund { get; set; }
        
        /// <summary>
        /// Indicates that Ledgers, created for Payments should not be confirmed automatically.
        /// </summary>
        public bool UnconfirmedLedger { get; set; }
        
        /// <summary>
        /// Indicated that Payment provider supports generating Mandate reference by billwerk.
        /// </summary>
        public bool SupportExternalMandateReference { get; set; }
        
        /// <summary>
        /// When SupportExternalMandateReference defines size limitations for generated reference.
        /// </summary>
        public int SupportedExternalMandateReferenceSize { get; set; }

        /// <summary>
        /// When SupportExternalMandateReference defines lower case limitations for generated reference.
        /// </summary>
        public bool SupportLowerCaseMandateReference { get; set; }
    }
}

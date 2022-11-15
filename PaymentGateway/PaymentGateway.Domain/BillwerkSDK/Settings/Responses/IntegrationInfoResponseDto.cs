using Billwerk.Payment.SDK.DTO.IntegrationInfo;

namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses
{
    /// <summary>
    /// Represent information about merchant integration.
    /// Tells billwerk how to work with it.
    /// </summary>
    public class IntegrationInfoResponseDto
    {
        /// <summary>
        /// Needed info for billwerk about setup of CC payment method.
        /// </summary>
        public PaymentMethodInfoDto CreditCardMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of DD payment method.
        /// </summary>
        public PaymentDirectDebitMethodInfoDto DebitMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of MobilePay payment method only for now.
        /// Experimental.
        /// </summary>
        public PaymentMethodInfoDto BlackLabelMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of On Account payment method.
        /// </summary>
        public PaymentMethodInfoDto OnAccountMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of Betalingsservice payment method.
        /// </summary>
        public PaymentDirectDebitMethodInfoDto BetalingsserviceMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of Autogiro payment method.
        /// </summary>
        public PaymentMethodInfoDto AutogiroMethodInfo { get; set; }

        /// <summary>
        /// Needed info for billwerk about setup of Avtalegiro payment method.
        /// </summary>
        public PaymentMethodInfoDto AvtalegiroMethodInfo { get; set; }

        /// <summary>
        /// Flag if integration used DataConfirmation flow.
        /// </summary>
        public bool UsePaymentDataConfirmationFlow { get; set; } = false;
        
        /// <summary>
        /// Flag if integration used DataConfirmation flow for preauth.
        /// </summary>
        public bool UsePaymentDataConfirmationFlowForPreauth { get; set; }
        
        /// <summary>
        /// Flag to Indicate does Integration support Refunds for all methods.
        /// Experimental, for billwerk only.
        /// </summary>
        public bool SupportRefunds { get; set; }
        
        /// <summary>
        /// Flag to Indicate should billwerk care about escalation process.
        /// </summary>
        public bool? SupportEscalations { get; set; }

        /// <summary>
        /// Flag which do not allow recurring before captures.
        /// </summary>
        public bool PreventRecurringBeforeCapture { get; set; }

        /// <summary>
        /// Flag to indicate that integration used scheduled payments
        /// Should not be a part of documentation, only for internal implementations
        /// For 3rd party scheduling should be implemented on their end.
        /// </summary>
        public bool UsesScheduledPayment { get; set; }

        /// <summary>
        /// Defines that integration supports overpayments per transaction.
        /// </summary>
        public bool SupportMultipleTransactionOverpayments { get; set; }
        
        /// <summary>
        /// Specifies that ReturnUrl should as a part of payment data of initial payment.
        /// </summary>
        public bool RequiresReturnUrl { get; set; }

        /// <summary>
        /// List of Settings Key to be filled by merchant and options how to build UI for particular item.
        /// </summary>
        public List<MerchantSettingDescription> MerchantSettings { get; set; }

        /// <summary>
        /// Flag to indicate that Initial Bearer should be provided.
        /// </summary>
        public bool HasSupportInitialBearer { get; set; }
        
        /// <summary>
        /// Flag to indicate that Order Data required for Initial payment.
        /// OrderDTO will be provided in initial preauth request.
        /// </summary>
        public bool HasSupportOrderDataInPreauth { get; set; }
        
        /// <summary>
        /// Flag to indicate that Invoice Data required for Initial payment.
        /// InvoiceDTO will be provided in initial preauth request.
        /// </summary>
        public bool HasSupportInvoiceData { get; set; }
    }
}

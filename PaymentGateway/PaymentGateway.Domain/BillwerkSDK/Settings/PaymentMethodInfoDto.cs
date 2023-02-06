namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo
{
    /// <summary>
    /// Specify billwerk payment process behavior.
    /// </summary>
    public class PaymentMethodInfoDto
    {
        /// <summary>
        /// Flag to Indicate that CapturePreauth should be used.
        /// </summary>
        public bool UseCapturePreauth { get; set; }
        
        /// <summary>
        /// Flag to Indicate that CancelPreauth should be used.
        /// </summary>
        public bool UseCancelPreauth { get; set; }

        /// <summary>
        /// Minimal Preauth Amount if specified.
        /// </summary>
        public decimal? DefaultPreauthAmount { get; set; }
        
        public bool HasSupportInitialBearer { get; set; }
    }
}

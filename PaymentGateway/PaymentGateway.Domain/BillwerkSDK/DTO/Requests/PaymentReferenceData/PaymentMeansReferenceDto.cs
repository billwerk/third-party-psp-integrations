using Billwerk.Payment.SDK.Enums;

namespace Billwerk.Payment.SDK.DTO.Requests.PaymentReferenceData
{
    /// <summary>
    /// Represents the payment means ref data.
    /// It include needed info for initial preauth request.
    /// </summary>
    public class PaymentMeansReferenceDto
    {
        public PaymentMeansReferenceDto()
        {
        }

        /// <summary>
        /// Instantiates a <seealso cref="PaymentMeansReferenceDto"/> for preauth with the provided parameters.
        /// This ctor used by preauth only. Exception - PayEx CC recurring flow.
        /// </summary>
        /// <param name="role">The role of a payment provider</param>
        /// <param name="successUrl">The finalization URL to redirect the payer to if payment result is successful</param>
        /// <param name="errorUrl">The finalization URL to redirect the payer to if payment failed</param>
        /// <param name="abortUrl">The finalization URL to redirect the payer to if payment aborted</param>
        public PaymentMeansReferenceDto(
            PublicPaymentProviderRole role,
            string successUrl,
            string errorUrl,
            string abortUrl)
        {
            Role = role;
            SuccessReturnUrl = successUrl;
            ErrorReturnUrl = errorUrl;
            AbortReturnUrl = abortUrl;
        }

        /// <summary>
        /// Instantiates a <seealso cref="PaymentMeansReferenceDto"/> for capture with the provided parameters
        /// </summary>
        /// <param name="role">The role of a payment provider</param>
        /// <param name="abortUrl">The finalization URL to redirect the payer to if payment aborted</param>
        public PaymentMeansReferenceDto(PublicPaymentProviderRole role, string abortUrl)
        {
            Role = role;
            AbortReturnUrl = abortUrl;
        }

        /// <summary>
        /// The bearer that represents customer payment data needed for the initial call in key/value format.
        /// Optional.
        /// </summary>
        public IDictionary<string, string> InitialBearer { get; set; }

        /// <summary>
        /// The payment provider role.
        /// Mandatory.
        /// </summary>
        public PublicPaymentProviderRole Role { get; set; }

        /// <summary>
        /// The finalization URL to redirect the payer to if payment result is successful.
        /// Optional.
        /// </summary>
        public string SuccessReturnUrl { get; set; }

        /// <summary>
        /// The finalization URL to redirect the payer to if payment failed.
        /// Optional.
        /// </summary>
        public string ErrorReturnUrl { get; set; }

        /// <summary>
        /// The finalization URL to redirect the payer to if payment aborted.
        /// Optional.
        /// </summary>
        public string AbortReturnUrl { get; set; }

        #region Experimental / billwerk specific fields

        /// <summary>
        /// The base host URL.
        /// Experimental &amp; billwerk specific (PayEx CC impl). Can not be used by 3-rd party integrator directly.
        /// </summary>
        public string HostUrl { get; set; }
        
        /// <summary>
        /// The URL to the terms of service document the payer must accept in order to complete the payment.
        /// Experimental &amp; billwerk specific (PayEx CC impl). Can not be used by 3-rd party integrator directly.
        /// </summary>
        public string TermsOfServiceUrl { get; set; }

        #endregion
    }
}

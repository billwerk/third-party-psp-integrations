using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;

namespace Billwerk.Payment.SDK.DTO.Responses
{
    /// <summary>
    /// Represents response data of agreement cancellation call from billwerk to Integrator.
    /// </summary>
    public class AgreementCancellationResponseDto : IResponseDto
    {
        /// <summary>
        /// The identifier of agreement to be cancelled.
        /// Mandatory.
        /// </summary>
        public string AgreementId { get; set; }

        /// <summary>
        /// The status of agreement cancellation.
        /// Mandatory.
        /// </summary>
        public AgreementCancellationStatus CancellationStatus { get; set; }
    }
}

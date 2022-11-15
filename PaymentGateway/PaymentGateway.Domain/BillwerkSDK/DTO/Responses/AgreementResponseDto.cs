using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;

namespace Billwerk.Payment.SDK.DTO.Responses
{
    /// <summary>
    /// Represents response data of get agreement call from billwerk to Integrator.
    /// </summary>
    public class AgreementResponseDto : IResponseDto
    {
        /// <summary>
        /// The bearer that represents customer settings payment data taken from PSP side.
        /// Optional.
        /// </summary>
        public Dictionary<string, string> Bearer { get; set; }
        
        /// <summary>
        /// The currency of agreement.
        /// Optional.
        /// Used for validation of currency during migration via technical bearer.
        /// ISO-4217 format
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Status of agreement.
        /// Mandatory.
        /// </summary>
        public AgreementStatus Status { get; set; }

        public PaymentErrorDto Error { get; set; }
    }
}

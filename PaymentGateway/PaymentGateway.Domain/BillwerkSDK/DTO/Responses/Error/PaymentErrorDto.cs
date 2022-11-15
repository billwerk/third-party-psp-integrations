using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;

namespace Billwerk.Payment.SDK.DTO.Responses.Error
{
    /// <summary>
    /// Applying format for errors from external API regarding any payment request.
    /// </summary>
    public class PaymentErrorDto : IResponseDto
    {
        
        /// <summary>
        /// Specific ErrorCode which must be mapped from PaymentErrorCode enum.
        /// Optional.
        /// </summary>
        public PaymentErrorCode ErrorCode { get; set; }
        
        /// <summary>
        /// Description of error. 
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Error code which come from PSP side.
        /// </summary>
        public string PspErrorCode { get; set; }
        
        /// <summary>
        /// Date time of the receiving error. 
        /// DateTime UTC format.
        /// Mandatory.
        /// </summary>
        public DateTime ReceivedAt { get; set; }

        /// <summary>
        /// Status of transaction regarding occured error. Can be Failed, Canceled or Expired.
        /// Optional. Failed by default.
        /// </summary>
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Failed;

        public override string ToString() =>
            $"{nameof(ErrorMessage)}: {ErrorMessage}, {nameof(PspErrorCode)}:" +
            $" {PspErrorCode}, {nameof(ErrorCode)}: {ErrorCode}, " +
            $"{nameof(Status)} : {Status}";
    }
}

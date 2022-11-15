using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Responses;

/// <summary>
/// Represents response data of payment cancellation call from billwerk to Integrator.
/// </summary>
public class PaymentCancellationResponseDto : IResponseDto
{
    /// <summary>
    /// The status of payment cancellation.
    /// Mandatory.
    /// </summary>
    public PaymentCancellationStatus CancellationStatus { get; set; }

    /// <summary>
    /// The identifier of billwerk transaction to be cancelled, used for consistency.
    /// Mandatory.
    /// </summary>
    public string TransactionId { get; set; }
}

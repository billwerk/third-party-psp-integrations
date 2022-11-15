using Billwerk.Payment.SDK.DTO.Responses.Payment;
using NodaTime;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Responses.Refund;

/// <summary>
/// billwerk inner model for inner external PSP impl.
/// </summary>
public class RefundItemDto : IPaymentItemDto
{
    /// <summary>
    /// The value taken from PSP side to identify a linked refund transaction.
    /// Optional.
    /// </summary>
    public string ExternalItemId { get; set; }
        
    /// <summary>
    /// Refund transaction amount.
    /// Mandatory.
    /// Positive number.
    /// </summary>
    public decimal Amount { get; set; }
        
    /// <summary>
    /// The date of a refund transaction on PSP side.
    /// Mandatory.
    /// </summary>
    public LocalDate BookingDate { get; set; }
        
    /// <summary>
    /// The description of a refund.
    /// Optional.
    /// </summary>
    public string Description { get; set; }
}

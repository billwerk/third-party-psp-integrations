using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Requests;

public class PaymentCancellationRequestDto : IRequestDto
{
    /// <summary>
    /// The identifier of PSP settings to identify a PSP module.
    /// Mandatory.
    /// <b>Not used now, since part of not ended integration flow with billwerk.</b>
    /// </summary>
    public string PspSettingsId { get; set; }
}

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.PSP;

public interface IPspRefundHandler : IPspHandler
{
    Task<Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto>> SendRefundAsync(NotEmptyString pspPaymentId, RefundRequestDto refundRequestDto, IDictionary<string, string> pspTransactionData);
}

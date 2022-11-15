using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.PSP;

public interface IPspPreauthHandler : IPspHandler
{
    Task<Result<InitialResponse, PaymentErrorDto>> SendInitialPreauthAsync(PreauthRequestDto preauthRequestDto);

    Task<Result<InitialResponse, PaymentErrorDto>> SendUpgradePreauthAsync(PreauthRequestDto preauthRequestDto, Domain.Modules.Transactions.Agreement.Agreement agreement);

    Task<Result<PaymentCancellationResponseDto, PaymentErrorDto>> CancelPreauthAsync(NotEmptyString? pspTransactionId);
}

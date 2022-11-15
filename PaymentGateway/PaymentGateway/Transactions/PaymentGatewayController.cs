using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Interfaces;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Transactions;

public class PaymentGatewayController : ControllerBase
{
    protected NotEmptyString DefaultPspSettingsId { get; }

    public PaymentGatewayController(ISettingsRepository settingsRepository)
        => DefaultPspSettingsId = (settingsRepository.GetDefault() as Domain.Modules.PSP.Settings.PspSettings).Id.ToString()
            .To(x => new NotEmptyString(x));
    
    protected ActionResult<TResponse> ResponseByResult<TResponse>(Result<TResponse, PaymentErrorDto> result) where TResponse : IResponseDto 
        => result.IsSuccess ? Ok(result.Data) : ResponseByPaymentErrorDto<TResponse>(result.Error);

    private ActionResult<TData> ResponseByPaymentErrorDto<TData>(PaymentErrorDto paymentErrorDto)
    {
        switch (paymentErrorDto.ErrorCode)
        {
            case PaymentErrorCode.UnmappedError:
            case PaymentErrorCode.AlreadyExecuted:
            case PaymentErrorCode.RateLimit:
            case PaymentErrorCode.InvalidPreconditions:
            case PaymentErrorCode.InternalError:
            case PaymentErrorCode.InvalidConfiguration:
            case PaymentErrorCode.PermissionDenied:
            case PaymentErrorCode.PSPConnectionTimeout:
            case PaymentErrorCode.InternalProviderError:
            case PaymentErrorCode.PSPConnectionProblem:
                return StatusCode(StatusCodes.Status500InternalServerError, paymentErrorDto);
            case PaymentErrorCode.LimitExceeded:
                return StatusCode(StatusCodes.Status429TooManyRequests, paymentErrorDto);
            case PaymentErrorCode.LoginError:
                return Unauthorized(paymentErrorDto);
            case PaymentErrorCode.BearerInvalid:
            case PaymentErrorCode.BearerExpired:
            case PaymentErrorCode.InvalidData:
            case PaymentErrorCode.InvalidAmount:
            case PaymentErrorCode.InvalidCountry:
            case PaymentErrorCode.InvalidCurrency:
            case PaymentErrorCode.InvalidBic:
            case PaymentErrorCode.InvalidIBAN:
            case PaymentErrorCode.InvalidNationalIdNumber:
            case PaymentErrorCode.InsufficientBalance:
            case PaymentErrorCode.Rejected:
            case PaymentErrorCode.Canceled:
                return UnprocessableEntity(paymentErrorDto);
            default: throw new ArgumentOutOfRangeException(nameof(paymentErrorDto.ErrorCode), $"Unknown error code {paymentErrorDto.ErrorCode}");
        }
    }
}

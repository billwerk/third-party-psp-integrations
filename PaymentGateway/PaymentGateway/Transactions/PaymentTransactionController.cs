using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.Transactions.Payment.FetchPayment;
using PaymentGateway.Application.Modules.Transactions.Payment.SendPayment;
using PaymentGateway.Application.Modules.Transactions.Preauth.CapturePreauth;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using PaymentGateway.Swagger.Payment;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Transactions;

[AllowAnonymous]
[ApiController]
[Route("/api/Payment")]
[Produces("application/json")]
public class PaymentTransactionController : PaymentGatewayController
{
    private readonly IMediator _mediator;

    public PaymentTransactionController(IMediator mediator, ISettingsRepository settingsRepository) : base(settingsRepository) 
        => _mediator = mediator;

    [HttpPost]
    [SwaggerRequestExample(typeof(PaymentRequestDto), typeof(PaymentRequestExamples))]
    [SwaggerResponseExample(201, typeof(PaymentResponseDto))]
    public async Task<ActionResult<PaymentResponseDto>> SendPayment([FromBody] PaymentRequestDto paymentRequestDto)
    {
        IRequest<Result<PaymentResponseDto, PaymentErrorDto>> command = paymentRequestDto.PreauthTransactionId switch
        {
            null => new SendPaymentCommand(paymentRequestDto, PaymentProvider.Reepay, DefaultPspSettingsId),
            not null => new CapturePreauthCommand(paymentRequestDto, PaymentProvider.Reepay, DefaultPspSettingsId),
        };

        return (await _mediator.Send(command)).To(ResponseByResult);
    }

    [HttpGet("{paymentTransactionId}")]
    [SwaggerResponseExample(200, typeof(PaymentResponseDto))]
    public async Task<ActionResult<PaymentResponseDto>> FetchPayment(string paymentTransactionId) =>
        (await new FetchPaymentCommand(new NotEmptyString(paymentTransactionId))
            .To(async command => await _mediator.Send(command)))
            .To(ResponseByResult);
}

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using PaymentGateway.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Modules.Transactions.Refund.FetchRefund;
using PaymentGateway.Application.Modules.Transactions.Refund.SendRefund;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Swagger.Refund;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Transactions;

[AllowAnonymous]
[ApiController]
[Route("/api/Refund")]
[Produces("application/json")]
public class RefundTransactionController : PaymentGatewayController
{
    private readonly IMediator _mediator;

    public RefundTransactionController(IMediator mediator, ISettingsRepository settingsRepository) : base(settingsRepository)
        => _mediator = mediator;
    
    [HttpPost]
    [SwaggerRequestExample(typeof(RefundRequestDto), typeof(RefundRequestExample))]
    [SwaggerResponseExample(201, typeof(RefundResponseExample))]
    public async Task<ActionResult<RefundResponseDto>> SendRefund([FromBody] RefundRequestDto refundRequestDto) =>
        (await new SendRefundCommand(refundRequestDto, DefaultPspSettingsId)
            .To(async command => await _mediator.Send(command)))
        .To(ResponseByResult);
    
    [HttpGet("{refundTransactionId}")]
    [SwaggerResponseExample(200, typeof(RefundResponseExample))]
    public async Task<ActionResult<RefundResponseDto>> FetchRefund(string refundTransactionId) =>
        (await new FetchRefundCommand(new NotEmptyString(refundTransactionId))
            .To(async command => await _mediator.Send(command)))
        .To(ResponseByResult);
}

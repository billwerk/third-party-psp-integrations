using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.Transactions.Preauth.CancelPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.FetchPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.FinalizePreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.SendInitialPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.SendUpgradePreauth;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Infrastructure;
using PaymentGateway.Shared;
using PaymentGateway.Swagger.Preauth;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Transactions;

[AllowAnonymous]
[ApiController]
[Route("/api/Preauth")]
[Produces("application/json")]
public class PreauthTransactionsController : PaymentGatewayController
{
    private readonly IMediator _mediator;

    public PreauthTransactionsController(IMediator mediator, ISettingsRepository settingsRepository) : base(settingsRepository) 
        => _mediator = mediator;

    [HttpPost]
    [SwaggerRequestExample(typeof(PreauthRequestDto), typeof(SendPreauthRequestExamples))]
    [SwaggerResponseExample(201 , typeof(SendPreauthOrFetchCurrentStateResponseExample))]
    public async Task<ActionResult<PreauthResponseDto>> SendPreauth([FromBody] PreauthRequestDto preauthRequestDto)
    {
        IRequest<Result<PreauthResponseDto, PaymentErrorDto>> command = preauthRequestDto.IsInitial switch
        {
            true => new SendInitialPreauthCommand(preauthRequestDto, PaymentGatewaySettings.CurrentPaymentProvider, DefaultPspSettingsId),
            false => new SendUpgradePreauthCommand(preauthRequestDto, PaymentGatewaySettings.CurrentPaymentProvider, DefaultPspSettingsId),
        };

        return (await _mediator.Send(command)).To(ResponseByResult);
    }
    
    [HttpPost("{preauthTransactionId}/Cancel")]
    [SwaggerRequestExample(typeof(PaymentCancellationRequestDto), typeof(CancelPreauthRequestExample))]
    [SwaggerResponseExample(200, typeof(CancelPreauthResponseExample))]
    public async Task<ActionResult<PaymentCancellationResponseDto>> CancelPreauth(
        [FromBody] PaymentCancellationRequestDto requestDto,
        [FromRoute] string preauthTransactionId)
        => (await _mediator.Send(new CancelPreauthCommand(new NotEmptyString(preauthTransactionId)))).To(ResponseByResult);

    [HttpGet("{preauthTransactionId}")]
    [SwaggerResponseExample(200, typeof(SendPreauthOrFetchCurrentStateResponseExample))]
    public async Task<ActionResult<PreauthResponseDto>> FetchPreauth(string preauthTransactionId)
        => (await new FetchPreauthCommand(new NotEmptyString(preauthTransactionId))
                .To(async fetchPreauthCommand => await _mediator.Send(fetchPreauthCommand)))
            .To(ResponseByResult);

    /// <summary>
    /// Specific experimental endpoint, which used for redirection flow (PSP's with checkout pages, without initial bearers).
    /// Not a part of public endpoints set.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("Finalize")]
    public async Task<ActionResult<PreauthResponseDto>> FinalizePreauth([FromBody] FinalizePreauthRequestDto finalizeRequestDto)
        => (await new FinalizePreauthCommand(finalizeRequestDto)
                .To(finalizePreauthCommand => _mediator.Send(finalizePreauthCommand)))
            .To(ResponseByResult);
}

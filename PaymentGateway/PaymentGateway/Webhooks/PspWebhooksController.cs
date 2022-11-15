using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using PaymentGateway.Transactions;

namespace PaymentGateway.Webhooks;

[ApiController]
[AllowAnonymous]
[Route("/api/webhooks/")]
public class PspWebhooksController : PaymentGatewayController
{
    private readonly INotificationHandler _notificationHandler;

    public PspWebhooksController(INotificationHandler notificationHandler, ISettingsRepository settingsRepository) : base(settingsRepository)
        => _notificationHandler = notificationHandler;

    [HttpPost("reepay")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public Task<IActionResult> Reepay()
        => OkAndExecuteAsync(requestBody => _notificationHandler.HandleAsync(PaymentProvider.Reepay, new NotEmptyString(requestBody)));
    
    [HttpPost("fakeProvider")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public Task<IActionResult> FakeProvider()
        => OkAndExecuteAsync(requestBody => _notificationHandler.HandleAsync(PaymentProvider.FakeProvider, new NotEmptyString(requestBody)));

    // Return 200 OK immediately and prevent any other status code to be set on the response.
    // Afterwards, execute the action asynchronously.
    private async Task<IActionResult> OkAndExecuteAsync(Func<string, Task> executeAction)
    {
        // Read request first
        HttpContext.Request.EnableBuffering();
        var requestBody = await HttpContext.Request.Body.ReadAsStringAndResetReaderAsync();

        // Return 200 OK. This may close the connection depending on the web server (e.g. IIS)
        Response.StatusCode = (int)HttpStatusCode.OK;
        await Response.CompleteAsync();

        await executeAction(requestBody);

        return new EmptyResult();
    }
}

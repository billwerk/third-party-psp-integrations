// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Webhook;
using Flurl.Http;
using PaymentGateway.Application;
using PaymentGateway.Application.Notification.Billwerk;

namespace PaymentGateway.Infrastructure;

public class BillwerkWebhookWrapper : IBillwerkWebhookWrapper
{
    private readonly IFlurlClientFactory _clientFactory;

    public BillwerkWebhookWrapper(IFlurlClientFactory clientFactory) => _clientFactory = clientFactory;

    public async Task<IntegrationWebhookDto> SendWebhookAsync(IntegrationWebhookDto webhookDto, Uri webhookUrl)
    {
        using var client = _clientFactory.Create(webhookUrl.AbsoluteUri);
        await client.Request().SendJsonAsync(HttpMethod.Post, webhookDto);

        return webhookDto;
    }
}

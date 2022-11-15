// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Webhook;

namespace PaymentGateway.Application.Notification.Billwerk;

public interface IBillwerkWebhookWrapper
{
    Task<IntegrationWebhookDto> SendWebhookAsync(IntegrationWebhookDto webhookDto, Uri webhookUrl);
}

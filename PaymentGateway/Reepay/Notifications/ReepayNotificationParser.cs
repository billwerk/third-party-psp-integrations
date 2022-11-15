// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace Reepay.Notifications;

public class ReepayNotificationParser : INotificationParser
{
    private readonly ILogger<ReepayNotificationParser> _logger;

    public ReepayNotificationParser(ILogger<ReepayNotificationParser> logger) => _logger = logger;

    public PaymentProvider PaymentProvider => PaymentProvider.Reepay;
    public IReadOnlyCollection<INotification> Parse(string rawData)
    {
        try
        {
            return rawData.To(JsonConvert.DeserializeObject<ReepayNotification>)
                .To(notification => notification.ToTransactionNotification(rawData))
                .One();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed during trying parse notification from Reepay side: unexpected format.");
            return new FailedParsedNotification(PaymentProvider, rawData.To<NotEmptyString>()).One();
        }
    }
}

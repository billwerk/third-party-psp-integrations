// Copyright (c) billwerk GmbH. All rights reserved

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace FakeProvider.Notifications;

public class FakeProviderNotificationParser : INotificationParser
{
    
    private readonly ILogger<FakeProviderNotificationParser> _logger;

    public FakeProviderNotificationParser(ILogger<FakeProviderNotificationParser> logger) => _logger = logger; 

    public PaymentProvider PaymentProvider => PaymentProvider.FakeProvider;
    public IReadOnlyCollection<INotification> Parse(string rawData)
    {
        try
        {
            return rawData.To(JsonConvert.DeserializeObject<FakeProviderNotification>)
                .To(notification => notification.ToTransactionNotification(rawData))
                .One();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed during trying parse notification from Fake Provider: unexpected format.");
            return new FailedParsedNotification(PaymentProvider, rawData.To<NotEmptyString>()).One();
        }
    }
}

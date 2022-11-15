// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Notification.EntryPoint;

/// <inheritdoc />
public class NotificationHandler : INotificationHandler
{
    private readonly IReadOnlyList<ITypedNotificationHandler> _typedNotificationHandlers;
    private readonly IEnumerable<INotificationParser> _notificationParsers;

    public NotificationHandler(IReadOnlyList<ITypedNotificationHandler> typedNotificationHandlers, IEnumerable<INotificationParser> notificationParsers)
    {
        _typedNotificationHandlers = typedNotificationHandlers;
        _notificationParsers = notificationParsers;
    }

    /// <inheritdoc />
    public async Task HandleAsync(PaymentProvider provider, NotEmptyString rawData)
    {
        var (parsedNotifications, notParsedNotifications) = ParseRawDataToNotifications(rawData, provider);

        await parsedNotifications.ForEachAsync(HandleParsedTypedNotificationAsync);
        notParsedNotifications.ForEach(LogNotParsedNotifications);
    }

    private Task HandleParsedTypedNotificationAsync(INotification notification) => _typedNotificationHandlers
        .Single(typedNotificationHandler => typedNotificationHandler.NotificationType == notification.GetType())
        .HandleTypedNotificationAsync(notification);

    private (IList<INotification> parsedNotifications, IList<INotification> notParsedNotifications) ParseRawDataToNotifications(NotEmptyString rawData, PaymentProvider paymentProvider)
    {
        var notifications = _notificationParsers.Single(parser => parser.PaymentProvider == paymentProvider)
            .Parse(rawData)
            .ToLookup(n => n is not FailedParsedNotification)
            .Select(notifications => notifications.AsEnumerable())
            .ToList();

        var notParsedNotifications = notifications.SingleOrDefault(n => n.Any(x => x is FailedParsedNotification)).OrEmpty().ToList();
        var parsedNotifications = notifications.SingleOrDefault(n => n.Any(x => x is not FailedParsedNotification)).OrEmpty().ToList();
        return (parsedNotifications, notParsedNotifications);
    }

    private void LogNotParsedNotifications(INotification obj)
    {
        //TODO: IT-9811: Webhooks logging & mirroring mechanism
    }
}

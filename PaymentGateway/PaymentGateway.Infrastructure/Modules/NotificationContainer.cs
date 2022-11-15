// Copyright (c) billwerk GmbH. All rights reserved

using DryIoc;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Application.Notification.Types;
using PaymentGateway.Application.Notification.Types.Transaction;

namespace PaymentGateway.Infrastructure.Modules;

public static class NotificationContainer
{
    public static IRegistrator Notification(this IRegistrator registrator)
    {
        registrator.Register<INotificationHandler, NotificationHandler>();
        
        registrator.Register<ITypedNotificationHandler, TransactionNotificationHandler>();

        return registrator;
    }
}

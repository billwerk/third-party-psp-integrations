// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Notification.Billwerk;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Domain.Modules.Transactions;

namespace PaymentGateway.Application.Notification.Types;

/// <summary>
/// Successfully parsed notifications can be related to different type of objects on PSP side, which reflected in PG domain.
/// For each of them PG has different handle strategy.
/// </summary>
public interface ITypedNotificationHandler
{
    Type NotificationType { get; }

    Task HandleTypedNotificationAsync(INotification notification);
}

public abstract class TypedNotificationHandlerBase : ITypedNotificationHandler
{
    public abstract Type NotificationType { get; }

    protected readonly ITransactionRepository TransactionRepository;
    protected readonly IBillwerkWebhookWrapper BillwerkWebhookWrapper;

    protected TypedNotificationHandlerBase(ITransactionRepository transactionRepository, IBillwerkWebhookWrapper billwerkWebhookWrapper)
    {
        TransactionRepository = transactionRepository;
        BillwerkWebhookWrapper = billwerkWebhookWrapper;
    }
    
    public async Task HandleTypedNotificationAsync(INotification notification)
    {
        try
        {
            await HandleTypedNotificationAsyncInternal(notification);
        }
        catch (Exception exception)
        {
            LogUnhandledNotification(notification, exception);
        }
    }

    private void LogUnhandledNotification(INotification notification, Exception exception)
    {
        //TODO: IT-9811: Webhooks logging & mirroring mechanism
    }

    /// <summary>
    /// Template method used to define specific strategy of handling notification of concrete type.
    /// </summary>
    protected abstract Task HandleTypedNotificationAsyncInternal<T>(T notification)
        where T : INotification;
}

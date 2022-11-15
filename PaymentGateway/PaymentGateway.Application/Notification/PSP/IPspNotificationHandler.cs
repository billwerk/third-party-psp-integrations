// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Notification.Types.Transaction;

namespace PaymentGateway.Application.Notification.PSP;

public interface IPspNotificationHandler : IPspHandler
{
    Task<ITransactionNotificationHandlingResult> HandleTransactionNotificationAsync(TransactionNotification transactionNotification, Domain.Modules.Transactions.Transaction transaction); //We need to have a transaction because of the Mollie specific issue with refunds + PspSettingsId
}

// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.Types.Transaction;

public record TransactionNotification(
    PaymentProvider PaymentProvider,
    NotEmptyString RawData,
    ITypedNotification? TypedNotification,
    NotEmptyString PspTransactionId) : INotification;

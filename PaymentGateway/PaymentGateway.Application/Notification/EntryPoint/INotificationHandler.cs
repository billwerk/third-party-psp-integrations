// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.EntryPoint;

/// <summary>
/// Entry point of notification handling channel.
/// On this moment we have only raw, untyped data directly from some abstract notification source (webhooks, reports..).
/// Based on source we also know Payment Provider that provides this notifications.
/// </summary>
public interface INotificationHandler
{
    /// <summary>
    /// Handle scenario:
    /// 1. Parse notification to PSP-concrete structures &amp; identify their types (notification about Agreement or Transaction).
    /// 2. Try handle successfully parsed notifications.
    /// 3. Log not parsed notifications.
    /// </summary>
    Task HandleAsync(PaymentProvider provider, NotEmptyString rawData);
}

// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.EntryPoint;

/// <summary>
/// Any kind of notification, which comes to PG from PSP, in untyped-raw structure.
/// </summary>
public interface INotification
{
    /// <summary>
    /// Sender of notifications. We always know, based on any abstract notification source, from which PSP notifications come.
    /// </summary>
    PaymentProvider PaymentProvider { get; init; }

    /// <summary>
    /// Notifications in untyped-raw structure.
    /// </summary>
    NotEmptyString RawData { get; init; }
}

/// <summary>
/// Type for identifying notifications which PG can not parse for some reasons. Used for logging purpose.
/// </summary>
public record FailedParsedNotification(PaymentProvider PaymentProvider, NotEmptyString RawData) : INotification;

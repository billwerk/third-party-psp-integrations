// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Notification.EntryPoint;

namespace PaymentGateway.Application.Notification.PSP;

public interface INotificationParser : IPspHandler
{
    IReadOnlyCollection<INotification> Parse(string rawData);
}

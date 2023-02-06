// Copyright (c) billwerk GmbH. All rights reserved

using DryIoc;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Domain.Modules;
using Reepay.Handlers;
using Reepay.Notifications;

namespace Reepay;

public static class ReepayContainer
{
    public static IRegistrator Reepay(this IRegistrator registrator)
    {
        registrator.Register<IPspPreauthHandler, ReepayPreauthHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));
        registrator.Register<ISupportFinalizePreauth, ReepayPreauthHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));
        registrator.Register<ISupportCapturePreauth, ReepayPreauthHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));
        registrator.Register<IPspPaymentHandler, ReepayPaymentHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));
        registrator.Register<IPspRefundHandler, ReepayRefundHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));

        registrator.Register<INotificationParser, ReepayNotificationParser>(Reuse.Scoped, serviceKey: PaymentProvider.Reepay);
        registrator.Register<IPspNotificationHandler, ReepayNotificationHandler>(Reuse.Scoped, serviceKey: PaymentProvider.Reepay);
        
        registrator.Register<IPspSettingsHandler, ReepaySettingsHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.Reepay));
        
        return registrator;
    }
}

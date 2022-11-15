// Copyright (c) billwerk GmbH. All rights reserved

using DryIoc;
using FakeProvider.Handlers;
using FakeProvider.Notifications;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Domain.Modules;

namespace FakeProvider;

public static class FakeProviderContainer
{
    public static IRegistrator FakeProvider(this IRegistrator registrator)
    {
        registrator.Register<IPspPreauthHandler, FakeProviderPreauthHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));
        registrator.Register<IPspPaymentHandler, FakeProviderPaymentHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));
        registrator.Register<IPspRefundHandler, FakeProviderRefundHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));
        registrator.Register<ISupportCapturePreauth, FakeProviderPreauthHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));

        registrator.Register<INotificationParser, FakeProviderNotificationParser>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));
        registrator.Register<IPspNotificationHandler, FakeProviderNotificationHandler>(Reuse.Scoped, setup: Setup.With(PaymentProvider.FakeProvider));

        return registrator;
    }
}

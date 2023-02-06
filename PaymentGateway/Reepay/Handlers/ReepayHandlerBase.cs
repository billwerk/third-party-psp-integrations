// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using Reepay.Wrapper;

namespace Reepay.Handlers;

public abstract class ReepayHandlerBase : IPspHandler
{
    protected readonly ReepayWrapper Wrapper;
    protected readonly ReepaySettings Settings;

    protected ReepayHandlerBase(
        ISettingsRepository settingsRepository,
        IFlurlClientFactory flurlClientFactory)
    {
        Settings = (ReepaySettings)settingsRepository.GetDefault(PaymentProvider); 
        Wrapper = new ReepayWrapper(Settings, flurlClientFactory);
    }

    public PaymentProvider PaymentProvider => PaymentProvider.Reepay;
}

// Copyright (c) billwerk GmbH. All rights reserved

using DryIoc.ImTools;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using IFlurlClientFactory = PaymentGateway.Application.IFlurlClientFactory;

namespace PaymentGateway.Infrastructure;

public class PspFlurlClientFactory : IFlurlClientFactory
{
    private readonly Flurl.Http.Configuration.IFlurlClientFactory _flurlClientFactory;
    
    public PspFlurlClientFactory()
    {
        _flurlClientFactory = new DefaultFlurlClientFactory();
    }

    public IFlurlClient Create(Url url) => _flurlClientFactory.Get(url).Do(client => client.BaseUrl = url);
}

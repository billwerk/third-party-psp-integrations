// Copyright (c) billwerk GmbH. All rights reserved

using Flurl;
using Flurl.Http;

namespace PaymentGateway.Application;

public interface IFlurlClientFactory
{
    IFlurlClient Create(Url url);
}

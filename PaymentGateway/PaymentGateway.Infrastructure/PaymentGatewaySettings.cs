// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain;
using PaymentGateway.Domain.Modules;

namespace PaymentGateway.Infrastructure;

public class PaymentGatewaySettings : IPaymentGatewaySettings
{
    public static bool UseInMemoryStorage = true;

    public static PaymentProvider CurrentPaymentProvider = PaymentProvider.FakeProvider;
    
    public string Domain { get; init; }

    private readonly string _environmentUrl;
    public string EnvironmentUrl
    {
        get => string.Format(_environmentUrl, Domain);
        init => _environmentUrl = value;
    }

    private readonly string _apiUrl;
    public string ApiUrl
    {
        get => string.Format(_apiUrl, EnvironmentUrl);
        init => _apiUrl = value;
    }
}

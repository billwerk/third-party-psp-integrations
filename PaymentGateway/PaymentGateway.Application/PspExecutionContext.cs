// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules;

namespace PaymentGateway.Application;

public class PspExecutionContext
{
    public PaymentProvider CurrentPaymentProvider { get; init; }
}

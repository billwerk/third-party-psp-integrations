// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules.PSP.Enums;

namespace PaymentGateway.Domain.Modules.PSP.Settings;

public interface ICreditCardPspSettings
{
    public CreditCardTypes[] AvailableCreditCardTypes { get; init; }
}

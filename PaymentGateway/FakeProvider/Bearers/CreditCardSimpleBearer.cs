// Copyright (c) billwerk GmbH. All rights reserved

namespace FakeProvider.Bearers;

public class CreditCardSimpleBearer
{
    public string CardType { get; set; }

    public string Country { get; set; }

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Holder { get; set; }

    public string Last4CardNumber { get; set; }
}

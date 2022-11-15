// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Domain.BillwerkSDK.Enums
{
    public enum WebhookType
    {
        Preauth = 1,
        Payment = 2,
        Refund = 3,
        Agreement = 4,
        Mandate = 5,
    }
}

// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Shared.Abstractions.DataAccess.DatabaseObjects;

namespace PaymentGateway.Domain.Modules.PSP.Settings;

public class PspSettings : DatabaseObjectBase, IMerchantPspSettings
{
    public virtual PaymentProvider PaymentProvider { get; }
}

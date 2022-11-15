// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.Notifications;

/// <summary>
/// Reepay events which should be handled by PG.
/// All others should be ignored, but logged anyway.
/// </summary>
public struct ReepayEvents
{
    public const string InvoiceSettled = "invoice_settled";
    public const string InvoiceFailed = "invoice_failed";
    public const string InvoiceRefund = "invoice_refund";
}

// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.Helpers;

public static class ReepayConstants
{
    public const string
        ReepayApiUrl = "https://api.reepay.com/v1",
        ReepayCheckoutApiUrl = "https://checkout-api.reepay.com/v1",
        ChargeSessionUrl = "session/charge",
        RecurringSessionUrl = "session/recurring",
        ChargeBaseUrl = "charge",
        SettleUrl = "settle",
        InvoiceBaseUrl = "invoice",
        TransactionSegmentUrl = "transaction",
        RefundBaseUrl = "refund",
        PaymentMethodsBaseUrl = "payment_method",
        PspTransactionDataInvoiceHandle = "invoiceHandle",
        WebhookSettingsUrl = "account/webhook_settings";
}

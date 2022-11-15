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
        InvoiceListBaseUrl = "list/invoice",
        RefundBaseUrl = "refund",
        PaymentMethodsBaseUrl = "payment_method",
        PspTransactionDataInvoiceHandle = "invoiceHandle";

    public const int InvoiceListMaxCount = 100;
}

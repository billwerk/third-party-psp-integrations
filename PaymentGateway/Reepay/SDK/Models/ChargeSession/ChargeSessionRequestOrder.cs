// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.SDK.Models.ChargeSession;

/// <summary>
/// https://docs.reepay.com/reference/createchargesession
/// </summary>
/// <param name="Handle">Per account unique reference to charge/invoice. E.g. order id from own system. Multiple payments can be attempted for the same handle but only one authorized or settled charge can exist per handle. Max length 255 with allowable characters [a-zA-Z0-9_.-@]. It is recommended to use a maximum of 20 characters as this will allow for the use of handle as reference on bank statements without truncation.</param>
/// <param name="Amount">Amount in the smallest unit. Either amount or order_lines must be provided if charge/invoice does not already exists.</param>
/// <param name="Currency">Optional currency in ISO 4217 three letter alpha code. If not provided the account default currency will be used. The currency of an existing charge or invoice cannot be changed.</param>
/// <param name="Source">The source for the payment. Either an existing payment method for the customer or a card token ct_.... The existing payment method can either be referenced directly with id, e.g. ca_..., or the keyword auto can be given to indicate that the newest active customer payment method should be used.</param>
/// <param name="Customer">An alternative to using a reference to an already created customer either by the customer_handle parameter or implicitly given if using a saved payment method as source. If this object is provided and the customer already exists, the existing customer will be used. Notice that customer cannot be changed for existing charge/invoice so if handle is provided it must match the customer handle for existing customer.</param>
public record ChargeSessionRequestOrder(
    string Handle,
    int Amount,
    string Currency,
    string Source,
    ChargeSessionRequestCustomer Customer);

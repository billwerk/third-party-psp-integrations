// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.SDK.Models.Refund;

/// <summary>
/// https://reference.reepay.com/api/#create-refund
/// </summary>
/// <param name="Invoice">Handle or id for invoice/charge to refund. Required</param>
/// <param name="Key">Optional idempotency key. Only one refund can be performed for the same key. An idempotency key identifies uniquely the request and multiple requests with the same key and invoice will yield the same result. In case of networking errors the same request with same key can safely be retried. Optional</param>
/// <param name="Amount">Optional amount in the smallest unit for the account currency. Either amount or note_lines can be provided, if neither is provided the full refundable amount is refunded. Optional</param>
public record CreateRefundRequest(
    string Invoice,
    int Amount,
    string? Key = null);

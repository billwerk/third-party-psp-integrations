// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using PaymentGateway.Domain.Modules.Transactions.Refund;

namespace PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;

public static class RefundStateBuilder
{
    public static RefundTransactionState Build(RefundResponseDto refundResponseDto)
        => new(refundResponseDto.Status, DateTime.UtcNow);
}

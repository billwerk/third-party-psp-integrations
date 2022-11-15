// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;

namespace PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;

public static class FailedStateBuilder
{
    public static TransactionErrorState Build(PaymentErrorDto paymentErrorDto)
        => new(paymentErrorDto.ReceivedAt, paymentErrorDto.ErrorMessage, paymentErrorDto.ErrorCode, paymentErrorDto.PspErrorCode);
}

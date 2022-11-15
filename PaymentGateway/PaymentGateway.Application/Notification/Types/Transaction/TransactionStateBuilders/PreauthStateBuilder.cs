// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;

public static class PreauthStateBuilder
{
    public static PreauthTransactionState Build(PreauthResponseDto preauthResponseDto) =>
        new(new NonNegativeAmount(preauthResponseDto.AuthorizedAmount),
            preauthResponseDto.Status,
            DateTime.UtcNow);
}

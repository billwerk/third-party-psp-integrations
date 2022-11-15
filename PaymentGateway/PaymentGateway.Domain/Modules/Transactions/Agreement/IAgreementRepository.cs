// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Agreement;

/// <summary>
/// Repository for managing <see cref="Agreement.Data">agreement data</see>
/// </summary>
/// <remarks>
/// - Not a part of <see cref="ITransactionRepository">TransactionRepository</see> because while
/// <see cref="Agreement">Agreement</see> is a part of <see cref="PreauthTransaction"/>,
/// it represents information for transactions and it is not a transaction itself<br/>
/// - Transactions are connected between each other with <see cref="Agreement.Id">AgreementId</see>,
/// e.g. AgreementId that transferred with payment request from <b>billwerk-platform</b> is used
/// to establish relation with related successful initial preauth transaction inside <b>payment-gateway</b><br/>
/// </remarks>
public interface IAgreementRepository
{
    Task<Agreement> GetAgreementByIdAsync(AgreementId agreementId);
}

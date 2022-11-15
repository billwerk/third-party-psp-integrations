// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions;

/// <summary>
/// Repository with basic actions for
/// <see cref="PreauthTransaction">Preauth</see>, <see cref="PaymentTransaction">Payment</see>, <see cref="RefundTransaction">Refund</see>
/// </summary>
/// <remarks>
/// In case if any specific extension is required for any transaction type
/// consider creation of a separate repository, e.g. <b>IPreauthTransactionRepository</b>
/// </remarks>
public interface ITransactionRepository 
{
    Task<Transaction> InsertAsync(Transaction transaction);
    
    Task<Transaction> UpdateAsync(Transaction transaction);

    Task<Transaction> GetByBillwerkTransactionIdAsync(BillwerkTransactionId id);

    Task<Transaction> GetSingleByPspTransactionIdAsync(NotEmptyString id);

    Task<Transaction?> GetInitialTransactionByPspAgreementIdAsync(NotEmptyString pspAgreementId);

    IEnumerable<Transaction> GetTransactionsByPspTransactionId(NotEmptyString id);
}

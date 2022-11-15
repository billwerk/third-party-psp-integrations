using MongoDB.Bson;
using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;

public record BillwerkTransactionId : ValueObject<string>
{
    public BillwerkTransactionId(string billwerkTransactionId)
    {
        Value = IsValidIdFormat(billwerkTransactionId) switch
        {
            true => billwerkTransactionId,
            false => throw new ValueObjectException<BillwerkTransactionId>($"Billwerk transaction id '{billwerkTransactionId}' has invalid format.")
        };
    }
    
    /// <summary>
    /// Inspect, is billwerk transaction id in valid format : {yyyyMMdd}-{ObjectId}
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    private bool IsValidIdFormat(string transactionId)
    {
        var objectIdPosition = transactionId.IndexOf("-", StringComparison.InvariantCulture) + 1;
        return objectIdPosition != -1 && ObjectId.TryParse(transactionId[objectIdPosition..], out var _);
    }
}

using MongoDB.Bson.Serialization;
using NodaTime;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public static class RegisterBsonSerializers
{
    private static void RegisterTypes()
    {
        BsonSerializer.RegisterSerializer(typeof(PositiveAmount), new PositiveAmountSerializer());
        BsonSerializer.RegisterSerializer(typeof(NonNegativeAmount), new NonNegativeAmountSerializer());
        BsonSerializer.RegisterSerializer(typeof(BillwerkTransactionId), new BillwerkTransactionIdSerializer());
        BsonSerializer.RegisterSerializer(typeof(Currency), new CurrencySerializer());
        BsonSerializer.RegisterSerializer(typeof(AgreementId), new AgreementIdSerializer());
        BsonSerializer.RegisterSerializer(typeof(NotEmptyString), new NotEmptyStringSerializer());
        BsonSerializer.RegisterSerializer(typeof(LocalDate), new LocalDateSerializer());

        BsonSerializer.RegisterSerializer(typeof(TransactionStateCollection<TransactionStateBase>), 
            new TransactionStateCollectionSerializer());
        BsonSerializer.RegisterSerializer(typeof(PreauthTransactionState), new PreauthTransactionStateSerializer());
        BsonSerializer.RegisterSerializer(typeof(TransactionErrorState), new TransactionErrorStateSerializer());
        BsonSerializer.RegisterSerializer(typeof(PaymentTransactionState), new PaymentTransactionStateSerializer());
        BsonSerializer.RegisterSerializer(typeof(RefundTransactionState), new RefundTransactionStateSerializer());

        BsonClassMap.RegisterClassMap<Agreement>(cm =>
        {
            cm.AutoMap();
            cm.GetMemberMap(x => x.BillwerkAgreementId).SetSerializer(new AgreementIdSerializer());
        });

        BsonClassMap.RegisterClassMap<Transaction>(cm =>
        {
            cm.AutoMap();
            cm.AddKnownType(typeof(PreauthTransaction));
            cm.AddKnownType(typeof(PaymentTransaction));
            cm.AddKnownType(typeof(RefundTransaction));
            cm.GetMemberMap(nameof(Transaction.PspTransactionId))
                .SetSerializer(new NotEmptyStringSerializer())
                .SetIgnoreIfNull(true);
        });

        BsonClassMap.RegisterClassMap<PaymentTransaction>(cm =>
        {
            cm.AutoMap();
            cm.GetMemberMap(nameof(PaymentTransaction.DueDate)).SetIgnoreIfNull(true);
        });

        // BsonClassMap.RegisterClassMap<PspSettings>(cm =>
        // {
        //     cm.AutoMap();
        //     cm.AddKnownType(typeof(ReepaySettings));
        // });
    }

    static RegisterBsonSerializers() => RegisterTypes();
    
    public static void EnsureRegistered()
    {
    }
}

using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Shared;
using NodaTime;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Payment;

public class PaymentTransactionState : TransactionStateBase
{
    public IReadOnlyCollection<PaymentItem> Payments { get; init; }

    public IReadOnlyCollection<RefundItem> Refunds { get; init; }

    public IReadOnlyCollection<ChargebackItem> Chargebacks { get; init; }
    
    public NonNegativeAmount PaidAmount => new(Payments.Sum(payment => payment.PositiveAmount));

    public NonNegativeAmount RefundedAmount => new(Refunds.Sum(refund => refund.PositiveAmount));

    public NonNegativeAmount ChargebackAmount => new(Chargebacks.Sum(chargeback => chargeback.Amount));
    
    public PaymentTransactionState(PaymentTransactionStatus status, DateTime receivedAt)
        : this(status, receivedAt, Array.Empty<PaymentItem>(), Array.Empty<RefundItem>(), Array.Empty<ChargebackItem>()) { }

    public PaymentTransactionState(
        PaymentTransactionStatus status,
        DateTime receivedAt,
        IReadOnlyCollection<PaymentItem> payments,
        IReadOnlyCollection<RefundItem> refunds,
        IReadOnlyCollection<ChargebackItem> chargebacks) : base(status, receivedAt)
    {
        Payments = payments;
        Refunds = refunds;
        Chargebacks = chargebacks;
    }

    public static PaymentTransactionState CreateInitialState() 
        => new(PaymentTransactionStatus.Initiated, DateTime.UtcNow);

    protected override bool EqualsInDetails(TransactionStateBase other) => other
        .To(state => (PaymentTransactionState)state)
        .To(state => Payments.Count == state.Payments.Count &&
                     Refunds.Count == state.Refunds.Count &&
                     Chargebacks.Count == state.Chargebacks.Count &&
                     PaidAmount == state.PaidAmount &&
                     RefundedAmount == state.RefundedAmount &&
                     ChargebackAmount == state.ChargebackAmount);
}

#region Payment Items

public record PaymentItem
{
    /// <summary>
    /// The value taken from PSP side to identify a linked payment transaction.
    /// Optional.
    /// </summary>
    public string PspItemId { get; init; }

    /// <summary>
    /// Payment transaction amount.
    /// Mandatory.
    /// Positive number.
    /// </summary>
    public PositiveAmount PositiveAmount { get; init; }

    /// <summary>
    /// The date of a payment transaction on PSP side.
    /// Mandatory.
    /// </summary>
    public LocalDate BookingDate { get; init; }

    /// <summary>
    /// The description of a payment transaction.
    /// Optional.
    /// </summary>
    public string Description { get; init; }
}

public record ChargebackItem
{
    /// <summary>
    /// The value taken from PSP side to identify a linked payment transaction.
    /// Optional.
    /// </summary>
    public string PspItemId { get; init; }
    
    /// <summary>
    /// The fee amount of chargeback.
    /// Optional.
    /// Positive number.
    /// </summary>
    public decimal FeeAmount { get; init; }
        
    /// <summary>
    /// The reason of chargeback.
    /// </summary>
    public PaymentChargebackReason Reason { get; init; }
        
    /// <summary>
    /// The reason of chargeback from PSP side.
    /// </summary>
    public string PspReasonCode { get; init; }
        
    /// <summary>
    /// Detailed chargeback reason description.
    /// </summary>
    public string PspReasonMessage { get; init; }

    /// <summary>
    /// Payment transaction amount.
    /// Mandatory.
    /// Positive number.
    /// </summary>
    public decimal Amount { get; init; }
        
    /// <summary>
    /// The date of a payment transaction on PSP side.
    /// Mandatory.
    /// </summary>
    public LocalDate BookingDate { get; init; }
        
    /// <summary>
    /// The description of a payment transaction.
    /// Optional.
    /// </summary>
    public string Description { get; init; }
}

public record RefundItem
{
    public string PspItemId { get; init; }
    
    public LocalDate BookingDate { get; init; }
    
    /// <summary>
    /// Refund transaction amount.
    /// Mandatory.
    /// Positive number.
    /// </summary>
    public PositiveAmount PositiveAmount { get; init; }
    
    /// <summary>
    /// The description of a refund transaction.
    /// Optional.
    /// </summary>
    public string Description { get; init; }
}

#endregion

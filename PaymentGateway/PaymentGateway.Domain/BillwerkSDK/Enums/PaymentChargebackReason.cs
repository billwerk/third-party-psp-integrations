namespace Billwerk.Payment.SDK.Enums
{
    /// <summary>
    /// The reason of a chargeback
    /// </summary>
    public enum PaymentChargebackReason
    {
        /// <summary>
        /// The reason is not defined
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// Chargeback is initiated by user
        /// </summary>
        UserInitiated = 2,
        /// <summary>
        /// Customer balance is insufficient (PayOne)
        /// </summary>
        InsufficientBalance = 3,
        /// <summary>
        /// Account expired or account no. / name not identical, incorrect or savings account (PayOne)
        /// </summary>
        BearerInvalid = 4,
        /// <summary>
        /// For PayOne the customer payment objection. For FarPay the balance is insufficient or customer rejected a payment
        /// </summary>
        Canceled = 5,
        /// <summary>
        /// Cannot be collected (PayOne)
        /// </summary>
        Rejected = 6
    }
}

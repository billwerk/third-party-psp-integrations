namespace Billwerk.Payment.SDK.Enums
{
    public enum PaymentCancellationStatus
    {
        /// <summary>
        /// Payment is not cancelled
        /// </summary>
        NotCancelled = -1,
        /// <summary>
        /// Cancellation is initiated
        /// </summary>
        Initiated = 1,
        /// <summary>
        /// Cancellation is in progress
        /// </summary>
        Pending = 2,
        /// <summary>
        /// Cancellation is successfully done
        /// </summary>
        Succeeded = 3,
        /// <summary>
        /// Cancellation failed
        /// </summary>
        Failed = 4,
    }
}

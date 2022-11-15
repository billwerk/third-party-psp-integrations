namespace Billwerk.Payment.SDK.Enums
{
    /// <summary>
    /// Represents a set of payment transaction statuses that will be mapped to Billwerk transaction statuses
    /// </summary>
    public enum PaymentTransactionStatus
    {
        /// <summary>
        /// Created locally and not transferred to the integration
        /// If a network error occured during transfer, keep it in this status and increment SyncFailureCount
        /// </summary>
        Initiated = 1,
        
        /// <summary>
        /// Transferred to integration
        /// Direct debit should remain in this status until the payment was confirmed
        /// </summary>
        Pending = 2,
        
        /// <summary>
        /// Only confirmed transactions should have Succeeded status
        /// A transaction should only have this set if the full amount was paid (or overpaid)
        /// </summary>
        Succeeded = 3,
        
        /// <summary>
        /// Payment transaction processed with error
        /// </summary>
        Failed = 4,
        
        /// <summary>
        /// Payment transaction was cancelled, rejected or declined
        /// </summary>
        Cancelled = 5,
        
        /// <summary>
        /// Payment expiration time period ended
        /// </summary>
        Expired = 6
    }
}

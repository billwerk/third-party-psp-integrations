namespace Billwerk.Payment.SDK.DTO.Requests.OrderData
{
    /// <summary>
    /// Represents the order related data.
    /// </summary>
    public class OrderDataDto
    {
        /// <summary>
        /// The plan name in billwerk for which this order created.
        /// Mandatory.
        /// </summary>
        public string PlanName { get; set; }
        
        /// <summary>
        /// The next date of a payment.
        /// Mandatory.
        /// </summary>
        public DateTimeOffset NextPaymentDate { get; set; }

        /// <summary>
        /// The order fee period taken from billwerk plan variant.
        /// Mandatory.
        /// </summary>
        public OrderDataPeriodDto FeePeriod { get; set; }
        
        /// <summary>
        /// The fee amount of recurring payment.
        /// Positive number.
        /// Mandatory.
        /// </summary>
        public decimal RecurringFee { get; set; }
        
        /// <summary>
        /// Id of contract from external system.
        /// </summary>
        public string ExternalContractId { get; set; }
    }
}

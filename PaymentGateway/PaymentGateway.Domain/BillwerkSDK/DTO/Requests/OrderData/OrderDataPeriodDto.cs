using Billwerk.Payment.SDK.Enums;

namespace Billwerk.Payment.SDK.DTO.Requests.OrderData
{
    /// <summary>
    /// Represents the order fee period data.
    /// </summary>
    public class OrderDataPeriodDto
    {
        /// <summary>
        /// The time period unit.
        /// Mandatory.
        /// </summary>
        public TimePeriod Unit { get; set; }

        /// <summary>
        /// The quantity of unit.
        /// Mandatory.
        /// Positive number.
        /// </summary>
        public int Quantity { get; set; }
    }
}

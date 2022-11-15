using System.ComponentModel;
using Billwerk.Payment.SDK.Enums;

namespace Billwerk.Payment.SDK.DTO.Requests.InvoiceData
{
    /// <summary>
    /// Reflection of billwerk line item model, using in invoices.
    /// </summary>
    public class InvoiceDataLineItemDto
    {
        /// <summary>
        /// Tax category for this line item.
        /// </summary>
        public InvoiceDataLineTaxCategory TaxCategory { get; set; }

        /// <summary>
        /// Description of line item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Total Gross of line item.
        /// </summary>
        public decimal TotalGross { get; set; }
        
        /// <summary>
        /// Total Net of line item.
        /// Could be nullable for summary item,
        /// where this amount cannot be calculated.
        /// </summary>
        public decimal? TotalNet { get; set; }
        
        /// <summary>
        /// Vat Percentage of line item.
        /// Could be nullable for items
        /// Where this amount cannot be filled. F.e. summary item
        /// </summary>
        public decimal? VatPercentage { get; set; }
        
        /// <summary>
        /// Line item period fee start.
        /// Should not be optional, except Summary line
        /// </summary>
        public DateTimeOffset? PeriodStart { get; set; }
        
        /// <summary>
        /// Line item period fee end.
        /// Should not be optional, except Summary line
        /// </summary>
        public DateTimeOffset? PeriodEnd { get; set; }
        
        /// <summary>
        ///Info taken from billwerk invoice, for not invoice\credit note source -NULL
        /// </summary>
        public decimal? PricePerUnit { get; set; }
        
        /// <summary>
        ///Info taken from billwerk invoice, for not invoice\credit note source -NULL
        /// </summary>
        public decimal? Quantity { get; set; }
    }
}

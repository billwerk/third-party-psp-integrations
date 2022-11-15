using NodaTime;

namespace Billwerk.Payment.SDK.DTO.Requests.InvoiceData
{
    /// <summary>
    /// Reflection of billwerk invoice model, including line items.
    /// </summary>
    public class InvoiceDataDto
    {
        /// <summary>
        /// Date of invoice creation.
        /// Mandatory.
        /// </summary>
        public LocalDate InvoiceDate { get; set; }

        /// <summary>
        /// Invoice due date.
        /// Mandatory.
        /// </summary>
        public LocalDate DueDate { get; set; }
        
        /// <summary>
        /// Any text info related to this invoice.
        /// Mandatory.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Set of invoice line items.
        /// Mandatory.
        /// </summary>
        public List<InvoiceDataLineItemDto> LineItems { get; set; }
    }
}

using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;

namespace Persistence.Models
{
    public class RefundTransaction : PaymentTransactionBase
    {
        public List<ExternalRefundItemDTO> Refunds { get; set; }
    }
}

using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Persistence.Mongo;

namespace Persistence.Models
{
    public class RefundTransaction : Transaction
    {
        public List<ExternalRefundItemDTO> Refunds { get; set; }
        
        public ObjectId<PaymentTransaction> PaymentTransactionId { get; set; }
    }
}

using System;
using Billwerk.Payment.SDK.DTO;

namespace Persistence.Models
{
    public class PreauthTransaction : PaymentTransactionBase
    {
        public decimal AuthorizedAmount { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public PaymentBearerDTO Bearer { get; set; }
    }
}
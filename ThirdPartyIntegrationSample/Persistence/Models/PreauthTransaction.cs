using System;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Interfaces.Models;

namespace Persistence.Models
{
    public class PreauthTransaction : Transaction, IPspPreauthTransaction
    {
        public decimal AuthorizedAmount { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public PaymentBearerDTO Bearer { get; set; }

        public string RecurringToken { get; set; }
    }
}
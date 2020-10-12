﻿using System;
using System.Collections.Generic;
using System.Linq;
using Billwerk.Payment.SDK.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    [BsonDiscriminator(Required = true)]
    [BsonKnownTypes(typeof(PreauthTransaction))]
    public abstract class PaymentTransaction : DbObject
    {
        protected PaymentTransaction()
        {
            StatusHistory = new List<PaymentTransactionNewStatus>();
        }

        public string ExternalTransactionId { get; set; }
        public string PspTransactionId { get; set; }
        public string Currency { get; set; }
        public decimal RequestedAmount { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<PaymentTransactionNewStatus> StatusHistory { get; set; }
        
        public int SequenceNumber { get; set; }

        [BsonIgnore]
        public PaymentTransactionNewStatus? Status =>
            StatusHistory.Count == 0 ? (PaymentTransactionNewStatus?) null : StatusHistory.Last();
    }
}
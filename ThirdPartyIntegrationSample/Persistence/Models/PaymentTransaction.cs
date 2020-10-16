using System;
using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class PaymentTransaction : PaymentTransactionBase
    {
        [BsonIgnoreIfNull]
        public ObjectId? PreauthTransactionId { get; set; }
        
        public List<ExternalPaymentChargebackItemDTO> Chargebacks { get; set; }

        public List<ExternalPaymentItemDTO> Payments { get; set; }

        // not sure if we want these refund related things here
        // Possible alternatives:
        // * A separate GET {transactionId}/refunds endpoint
        // * Simply looking at the refund transactions and summing them
        // * Include refund related information here
        public decimal RefundedAmount { get; set; }
        
        public decimal RefundableAmount { get; set; }

        // in the MVP this should only be filled for preauth with new payment data
        //Mandate data and so on can be updated later
        public PaymentBearerDTO Bearer { get; set;}
        
        public DateTime? DueDate { get; set; }
    }
}
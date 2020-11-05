using System;
using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Interfaces.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class PaymentTransaction : Transaction, IPspPaymentTransaction
    {
        [BsonIgnoreIfNull]
        public ObjectId? PreauthTransactionId { get; set; }
        
        public List<ExternalPaymentChargebackItemDTO> Chargebacks { get; set; }

        public List<ExternalPaymentItemDTO> Payments { get; set; }
        
        public decimal RefundedAmount { get; set; }
        
        public decimal RefundableAmount { get; set; }

        public PaymentBearerDTO Bearer { get; set;}
        
        public DateTime? DueDate { get; set; }
        
        public string InvoiceReferenceCode { get; set; }
        
        public string TransactionInvoiceReferenceText { get; set; }

        public string TransactionReferenceText { get; set; }
    }
}
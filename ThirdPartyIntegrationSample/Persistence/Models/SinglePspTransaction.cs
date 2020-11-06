using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Persistence.Models
{
    public class SinglePspTransaction
    {
        private readonly List<Transaction> _all = new List<Transaction>();
        private readonly List<RefundTransaction> _refunds;

        public SinglePspTransaction(Transaction transaction)
        {
            _all.Add(transaction);
        }

        public SinglePspTransaction(PreauthTransaction preauth, PaymentTransaction capture, List<RefundTransaction> refunds)
        {
            if (preauth != null)
            {
                _all.Add(preauth);
            }

            if (capture != null)
            {
                _all.Add(capture);
            }

            _refunds = refunds;
        }

        public Transaction GetLatest()
        {
            return _all.OrderByDescending(t => t.Id).FirstOrDefault();
        }

        public RefundTransaction GetRefundTransaction(string refundReference)
        {
            //TODO remove SequenceNumber from transaction data
            //It's PayOne specific
            var sequenceNumber = int.Parse(refundReference);
            return _refunds.SingleOrDefault(r => r.SequenceNumber == sequenceNumber);
        }

        public Transaction GetByExternalTransactionId(string transactionId)
        {
            return string.IsNullOrEmpty(transactionId) ? null : _all.SingleOrDefault(t => t.ExternalTransactionId == transactionId);
        }

        public static SinglePspTransaction GetFromIFindFluent(IFindFluent<Transaction, Transaction> findFluent)
        {
            var lst = findFluent.ToList();
            switch (lst.Count)
            {
                case 0:
                    return null;
                case 1:
                    return new SinglePspTransaction(lst[0]);
                default:
                {
                    PreauthTransaction preauth = null;
                    PaymentTransaction capture = null;
                    var refunds = new List<RefundTransaction>();
                    foreach (var transaction in lst)
                    {
                        if (transaction.GetType() == typeof(PreauthTransaction))
                            preauth = transaction as PreauthTransaction;
                        else if (transaction.GetType() == typeof(PaymentTransaction))
                            capture = transaction as PaymentTransaction;
                        else if (transaction.GetType() == typeof(RefundTransaction))
                            refunds.Add(transaction as RefundTransaction);
                    }
                    return new SinglePspTransaction(preauth, capture, refunds);
                }
            }
        }
    }
}
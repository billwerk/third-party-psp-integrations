using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Persistence.Models
{
    public class SinglePspTransaction
    {
        private readonly List<PaymentTransactionBase> _all = new List<PaymentTransactionBase>();

        public SinglePspTransaction(PaymentTransactionBase paymentTransaction)
        {
            _all.Add(paymentTransaction);
        }

        public SinglePspTransaction(PreauthTransaction preauth, PaymentTransaction capture)
        {
            _all.Add(preauth);
            _all.Add(capture);
        }

        public PaymentTransactionBase GetLatest()
        {
            return _all.OrderByDescending(t => t.Id).FirstOrDefault();
        }

        public PaymentTransactionBase GetByTransactionId(string transactionId)
        {
            return !ObjectId.TryParse(transactionId, out var trId) ? null : _all.SingleOrDefault(t => t.Id == trId);
        }

        public static SinglePspTransaction GetFromIFindFluent(IFindFluent<PaymentTransactionBase, PaymentTransactionBase> findFluent)
        {
            var lst = findFluent.ToList();
            switch (lst.Count)
            {
                case 0:
                    return null;
                case 1:
                    return new SinglePspTransaction(lst[0]);
                case 2:
                {
                    PreauthTransaction preauth = null;
                    PaymentTransaction capture = null;
                    foreach (var transaction in lst)
                    {
                        if (transaction.GetType() == typeof(PreauthTransaction))
                            preauth = transaction as PreauthTransaction;
                        else if (transaction.GetType() == typeof(PaymentTransaction))
                            capture = transaction as PaymentTransaction;
                    }
                    if (preauth!=null && capture!=null)
                        return new SinglePspTransaction(preauth, capture);
                    break;
                }
            }

            throw new InvalidOperationException($"It is not possible to create SinglePspTransaction from a cursor. Transactions in a cursor={lst.Count}.");
        }
    }
}
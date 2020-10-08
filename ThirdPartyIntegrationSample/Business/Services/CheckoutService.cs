﻿using Business.Interfaces;

namespace Business.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ITetheredPaymentInformationEncoder _paymentInformationEncoder;

        protected CheckoutService(ITetheredPaymentInformationEncoder paymentInformationEncoder)
        {
            _paymentInformationEncoder = paymentInformationEncoder;
        }

        public CheckoutResult Checkout(string json)
        {
            return new CheckoutResult(_paymentInformationEncoder.Encrypt(json));
        }
    }
}
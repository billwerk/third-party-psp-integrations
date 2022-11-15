// Copyright (c) billwerk GmbH. All rights reserved

using Reepay.SDK.Models.Charges;
using Reepay.SDK.Models.PaymentMethods;

namespace Reepay.Bearers;

internal record CreditCardPaymentBearer(
    string Country,
    string CardType,
    string MaskedCardPan,
    string Last4,
    int ExpiryMonth,
    int ExpiryYear,
    string RecurringPaymentMethod,
    string Customer)
{
    public CreditCardPaymentBearer(Charge charge) : this(charge.Source.CardCountry,
        charge.Source.CardType,
        charge.Source.MaskedCard,
        charge.Source.MaskedCard[^4..],
        int.Parse(charge.Source.ExpDate.Split('-')[0]),
        int.Parse(charge.Source.ExpDate.Split('-')[1]),
        charge.RecurringPaymentMethod,
        charge.Customer)
    {
    }

    public CreditCardPaymentBearer(PaymentMethod paymentMethod) : this(
        paymentMethod.Card.CardCountry!,
        paymentMethod.Card.CardType.ToString(),
        paymentMethod.Card.MaskedCard!,
        paymentMethod.Card.MaskedCard![^4..],
        int.Parse(paymentMethod.Card.ExpDate!.Split('-')[0]),
        int.Parse(paymentMethod.Card.ExpDate.Split('-')[1]),
        paymentMethod.Id,
        paymentMethod.Customer)
    {
    }
}

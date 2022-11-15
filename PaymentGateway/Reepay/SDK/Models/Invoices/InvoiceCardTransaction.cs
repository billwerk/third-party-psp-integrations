// Copyright (c) billwerk GmbH. All rights reserved

using Reepay.SDK.Enums;

namespace Reepay.SDK.Models.Invoices;

/// <summary>
/// https://reference.reepay.com/api/#cardtransaction
/// </summary>
/// <param name="CardType">Card type: unknown, visa, mc, dankort, visa_dk, ffk, visa_elec, maestro, laser, amex, diners, discover or jcb.</param>
/// <param name="ExpDate">Card expire date on form MM-YY.</param>
/// <param name="MaskedCard">Masked card number.</param>
/// <param name="CardCountry">Card issuing country in ISO 3166-1 alpha-2.</param>
/// <param name="Error">Error code if failed.</param>
/// <param name="ErrorState">Error state if failed: pending, soft_declined, hard_declined or processing_error.</param>
public record InvoiceCardTransaction(
    string CardType,
    string ExpDate,
    string MaskedCard,
    string CardCountry,
    TransactionError? Error,
    ErrorState? ErrorState);

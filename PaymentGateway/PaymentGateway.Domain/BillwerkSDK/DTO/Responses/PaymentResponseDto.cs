// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Payment;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Responses;

/// <summary>
/// Represents payment transaction data.
/// </summary>
public class PaymentResponseDto : ITransactionResponseDto
{
    /// <summary>
    /// The list of payment data specified on PSP side.
    /// Mandatory.
    /// Usually, for normal case, here should be one item.
    /// For specific cases, as partial payments, here can be several items.
    /// </summary>
    public List<PaymentItemDto> Payments { get; set; }

    /// <summary>
    /// The list of chargeback data specified on PSP side.
    /// Mandatory.
    /// </summary>
    public List<PaymentChargebackItemDto> Chargebacks { get; set; }
        
    /// <summary>
    /// The complete payment transaction already refunded amount.
    /// Mandatory.
    /// Positive or equaled to zero number.
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// The payment transaction amount to be refunded.
    /// Mandatory.
    /// Positive or equaled to zero number.
    /// (RefundedAmount + RefundableAmount) should be not greater than transaction paid amount.
    /// </summary>
    public decimal RefundableAmount { get; set; }

        
    /// <summary>
    /// The bearer that represents customer settings payment data taken from PSP side.
    /// Mandatory.
    /// </summary>
    public IDictionary<string, string> Bearer { get; set; }

    /// <summary>
    /// Planned date of a payment.
    /// Optional, applies to Direct Debit and Black Label payment methods only.
    /// </summary>
    public DateTime? DueDate { get; set; }
        
    /// <summary>
    /// Transaction identifier specified by the integrator.
    /// Mandatory.
    /// </summary>
    public string ExternalTransactionId { get; set; }

    /// <summary>
    /// Transaction identifier specified by the payment provider.
    /// Mandatory.
    /// </summary>
    public string PspTransactionId { get; set; }
        
    /// <summary>
    /// The currency of transaction.
    /// ISO-4217 format.
    /// Mandatory.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// The amount requested for the call.
    /// Positive number.
    /// Mandatory.
    /// </summary>
    public decimal RequestedAmount { get; set; }
        
    /// <summary>
    /// Date time of the last update. Used to prevent older versions overwriting by new ones.
    /// DateTime UTC format.
    /// Mandatory.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Payment transaction status.
    /// Mandatory.
    /// </summary>
    public PaymentTransactionStatus Status { get; set; }
        
    /// <summary>
    /// The identifier of billwerk payment transaction, used for consistency.
    /// Mandatory.
    /// </summary>
    public string TransactionId { get; set; }
}

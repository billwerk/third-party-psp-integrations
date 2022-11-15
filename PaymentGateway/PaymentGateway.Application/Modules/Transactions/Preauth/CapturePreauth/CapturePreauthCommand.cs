// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.CapturePreauth;

public class CapturePreauthCommand : IRequest<Result<PaymentResponseDto, PaymentErrorDto>>
{
    public CapturePreauthCommand(PaymentRequestDto paymentRequestDto, PaymentProvider paymentProvider, NotEmptyString pspSettingsId)
    {
        TransactionId = new BillwerkTransactionId(paymentRequestDto.TransactionId);
        PreauthTransactionId = paymentRequestDto.PreauthTransactionId?.To(id => new BillwerkTransactionId(id));
        RequestedPositiveAmount = new PositiveAmount(paymentRequestDto.RequestedAmount);
        Currency = new Currency(paymentRequestDto.Currency);
        WebhookTarget = new Uri(paymentRequestDto.WebhookTarget);
        TransactionReferenceText = paymentRequestDto.TransactionReferenceText;
        TransactionInvoiceReferenceText = paymentRequestDto.TransactionInvoiceReferenceText;
        PayerData = paymentRequestDto.PayerData;
        AgreementId = new AgreementId(paymentRequestDto.AgreementId);
        InitialPaymentRequestDto = paymentRequestDto;

        if (!string.IsNullOrWhiteSpace(paymentRequestDto.PaymentMeansReference.AbortReturnUrl))
            AbortReturnUrl = new Uri(paymentRequestDto.PaymentMeansReference.AbortReturnUrl);

        var paymentProviderRole = ProviderRoleMapper.FromPublicToInternal(paymentRequestDto.PaymentMeansReference.Role);
        PaymentMethodInfo = new PaymentMethodInfo(paymentProviderRole, paymentProvider);
        PspSettingsId = pspSettingsId;
    }
    public BillwerkTransactionId TransactionId { get; }

    public BillwerkTransactionId? PreauthTransactionId { get; }

    public PositiveAmount RequestedPositiveAmount { get; }

    public Currency Currency { get; }

    public Uri WebhookTarget { get; }

    public Uri AbortReturnUrl { get; }

    public string TransactionReferenceText { get; }

    public string TransactionInvoiceReferenceText { get; }

    public PayerDataDto PayerData { get; }

    public AgreementId AgreementId { get; }

    public PaymentMethodInfo PaymentMethodInfo { get; }

    public PaymentRequestDto InitialPaymentRequestDto { get; }

    public NotEmptyString PspSettingsId { get; }
}

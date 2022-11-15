using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Payment.SendPayment;

public class SendPaymentCommand : IRequest<Result<PaymentResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId TransactionId { get; }
    
    public BillwerkTransactionId? PreauthTransactionId { get; }
    
    public PositiveAmount RequestedPositiveAmount { get; }
    
    public Currency Currency { get; }
    
    public Uri WebhookTarget { get; }
    
    public PaymentMeansReference PaymentMeansReference { get; }
    
    public string TransactionReferenceText { get; }
    
    public string TransactionInvoiceReferenceText { get; }

    public PayerDataDto PayerData { get; }

    public AgreementId AgreementId { get; }

    public PaymentMethodInfo PaymentMethodInfo { get; }
    
    public PaymentRequestDto InitialPaymentRequestDto { get; }

    public NotEmptyString PspSettingsId { get; }

    public SendPaymentCommand(PaymentRequestDto paymentRequestDto, PaymentProvider paymentProvider, NotEmptyString pspSettingsId)
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

        PaymentMeansReference = new PaymentMeansReference
        {
            InitialBearer = paymentRequestDto.PaymentMeansReference.InitialBearer,
            AbortReturnUrl = new Uri(paymentRequestDto.PaymentMeansReference.AbortReturnUrl),
            SuccessReturnUrl = new Uri(paymentRequestDto.PaymentMeansReference.SuccessReturnUrl),
            ErrorReturnUrl = new Uri(paymentRequestDto.PaymentMeansReference.ErrorReturnUrl),
        };

        var paymentProviderRole = ProviderRoleMapper.FromPublicToInternal(paymentRequestDto.PaymentMeansReference.Role);
        PaymentMethodInfo = new PaymentMethodInfo(paymentProviderRole, paymentProvider);
        PspSettingsId = pspSettingsId;
    }
}

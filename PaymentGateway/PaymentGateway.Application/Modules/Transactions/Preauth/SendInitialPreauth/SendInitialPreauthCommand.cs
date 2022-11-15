using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Requests.OrderData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.SendInitialPreauth;

public class SendInitialPreauthCommand : IRequest<Result<PreauthResponseDto, PaymentErrorDto>>
{
    public SendInitialPreauthCommand(PreauthRequestDto preauthRequestDto, PaymentProvider paymentProvider, NotEmptyString pspSettingsId)
    {
        TransactionId = new BillwerkTransactionId(preauthRequestDto.TransactionId);
        RequestedNonNegativeAmount = new NonNegativeAmount(preauthRequestDto.RequestedAmount);
        Currency = new Currency(preauthRequestDto.Currency);
        WebhookTarget = new Uri(preauthRequestDto.WebhookTarget);
        TransactionReferenceText = preauthRequestDto.TransactionReferenceText;
        TransactionInvoiceReferenceText = preauthRequestDto.TransactionInvoiceReferenceText;
        PayerData = preauthRequestDto.PayerData;
        OrderData = preauthRequestDto.OrderData;
        AgreementId = new AgreementId(preauthRequestDto.AgreementId);
        
        PaymentMeansReference = new PaymentMeansReference
        {
            InitialBearer = preauthRequestDto.PaymentMeansReference.InitialBearer,
            AbortReturnUrl = new Uri(preauthRequestDto.PaymentMeansReference.AbortReturnUrl),
            SuccessReturnUrl = new Uri(preauthRequestDto.PaymentMeansReference.SuccessReturnUrl),
            ErrorReturnUrl = new Uri(preauthRequestDto.PaymentMeansReference.ErrorReturnUrl),
        };

        InitialAgreement = new Domain.Modules.Transactions.Agreement.Agreement
        {
            BillwerkAgreementId = AgreementId,
            PaymentBearer = PaymentMeansReference.InitialBearer,
        };

        var paymentProviderRole = ProviderRoleMapper.FromPublicToInternal(preauthRequestDto.PaymentMeansReference.Role);
        PaymentMethodInfo = new PaymentMethodInfo(paymentProviderRole, paymentProvider);
        
        InitialPreauthRequestDto = preauthRequestDto;
        PspSettingsId = pspSettingsId;
    }

    public BillwerkTransactionId TransactionId { get; }

    public NonNegativeAmount RequestedNonNegativeAmount { get; }

    public Currency Currency { get; }

    public Uri WebhookTarget { get; }

    public PaymentMeansReference PaymentMeansReference { get; }

    public string TransactionInvoiceReferenceText { get; }

    public string TransactionReferenceText { get; }

    public PayerDataDto PayerData { get; }

    public OrderDataDto OrderData { get; }

    public AgreementId AgreementId { get; }

    public PreauthRequestDto InitialPreauthRequestDto { get; }

    public PaymentMethodInfo PaymentMethodInfo { get; }

    public NotEmptyString PspSettingsId { get; }

    public Domain.Modules.Transactions.Agreement.Agreement InitialAgreement { get; }
}

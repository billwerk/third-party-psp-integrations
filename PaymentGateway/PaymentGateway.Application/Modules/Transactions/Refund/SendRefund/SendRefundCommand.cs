using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Refund.SendRefund;

public class SendRefundCommand : IRequest<Result<RefundResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId TransactionId { get; }
    
    public BillwerkTransactionId PaymentTransactionId { get; }
    
    public PositiveAmount Amount { get; }
    
    public Currency Currency { get; }
    
    public Uri WebhookTarget { get; }

    public RefundRequestDto InitialRefundRequestDto { get; }
    
    public NotEmptyString PspSettingsId { get; }

    public SendRefundCommand(RefundRequestDto refundRequestDto, NotEmptyString pspSettingsId)
    {
        TransactionId = new BillwerkTransactionId(refundRequestDto.TransactionId);
        PaymentTransactionId = new BillwerkTransactionId(refundRequestDto.PaymentTransactionId);
        Amount = new PositiveAmount(refundRequestDto.RequestedAmount);
        Currency = new Currency(refundRequestDto.Currency);
        WebhookTarget = new Uri(refundRequestDto.WebhookTarget);
        InitialRefundRequestDto = refundRequestDto;
        PspSettingsId = pspSettingsId;
    }
}

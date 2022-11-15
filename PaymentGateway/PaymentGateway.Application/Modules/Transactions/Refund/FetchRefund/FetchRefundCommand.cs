using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Refund.FetchRefund;

public class FetchRefundCommand : IRequest<Result<RefundResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId RefundTransactionId { get; }

    public FetchRefundCommand(NotEmptyString refundTransactionId)
    {
        RefundTransactionId = new BillwerkTransactionId(refundTransactionId);
    }
}

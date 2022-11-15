using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Payment.FetchPayment;

public class FetchPaymentCommand : IRequest<Result<PaymentResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId PaymentTransactionId { get; }

    public FetchPaymentCommand(NotEmptyString paymentTransactionId)
    {
        PaymentTransactionId = new BillwerkTransactionId(paymentTransactionId);
    }
}

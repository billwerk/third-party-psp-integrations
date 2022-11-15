using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.CancelPreauth;

public class CancelPreauthCommand : IRequest<Result<PaymentCancellationResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId TransactionId { get; }

    public CancelPreauthCommand(NotEmptyString transactionId)
    {
        TransactionId = new BillwerkTransactionId(transactionId);
    }
}

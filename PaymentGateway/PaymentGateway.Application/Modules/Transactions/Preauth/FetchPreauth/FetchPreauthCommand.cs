using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.FetchPreauth;

public class FetchPreauthCommand : IRequest<Result<PreauthResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId PreauthTransactionId { get; }

    public FetchPreauthCommand(NotEmptyString preauthTransactionId)
    {
        PreauthTransactionId = new BillwerkTransactionId(preauthTransactionId);
    }
}

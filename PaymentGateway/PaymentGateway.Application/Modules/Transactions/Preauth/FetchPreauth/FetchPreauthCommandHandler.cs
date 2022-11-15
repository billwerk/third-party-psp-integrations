using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.FetchPreauth;

public class FetchPreauthCommandHandler : IRequestHandler<FetchPreauthCommand, Result<PreauthResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public FetchPreauthCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    public async Task<Result<PreauthResponseDto, PaymentErrorDto>> Handle(FetchPreauthCommand request, CancellationToken cancellationToken)
    {
        var preauthTransaction = (await _transactionRepository.GetByBillwerkTransactionIdAsync(request.PreauthTransactionId)).To<PreauthTransaction>();

        if (preauthTransaction.States.Current is TransactionErrorState errorState)
            return BuildPaymentErrorDto(errorState);

        return Result<PreauthResponseDto, PaymentErrorDto>.Ok(preauthTransaction.To(MapPreauthTransactionToPreauthResponseDto));
    }

    private static Result<PreauthResponseDto, PaymentErrorDto> BuildPaymentErrorDto(TransactionErrorState errorState) => Result<PreauthResponseDto, PaymentErrorDto>.Failure(new PaymentErrorDto
    {
        ErrorCode = errorState.ErrorCode,
        ErrorMessage = errorState.ErrorMessage,
        PspErrorCode = errorState.PspErrorCode!,
        ReceivedAt = errorState.ReceivedAt,
    });

    private static PreauthResponseDto MapPreauthTransactionToPreauthResponseDto(PreauthTransaction preauthTransaction)
    {
        var preauthResponseDto = new PreauthResponseDto
        {
            ExternalTransactionId = preauthTransaction.Id.ToString(),
            TransactionId = preauthTransaction.BillwerkTransactionId.Value,
            PspTransactionId = preauthTransaction.PspTransactionId,
            Currency = preauthTransaction.Currency.Value,
            RequestedAmount = preauthTransaction.RequestedAmount,
            Bearer = preauthTransaction.Agreement.PaymentBearer,
        };
        
        var currentState = preauthTransaction.States.Current;
        preauthResponseDto.LastUpdated = currentState.LastSeenAt;
        preauthResponseDto.Status = currentState.Status;
        preauthResponseDto.AuthorizedAmount = currentState.Status == PaymentTransactionStatus.Failed 
            ? decimal.Zero
            : currentState.To<PreauthTransactionState>().AuthorizedAmount;
        
        return preauthResponseDto;
    }
}

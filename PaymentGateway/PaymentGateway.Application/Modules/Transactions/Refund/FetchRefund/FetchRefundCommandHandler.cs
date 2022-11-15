using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Refund.FetchRefund;

public class FetchRefundCommandHandler : IRequestHandler<FetchRefundCommand, Result<RefundResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public FetchRefundCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    public async Task<Result<RefundResponseDto, PaymentErrorDto>> Handle(FetchRefundCommand request, CancellationToken cancellationToken)
    {
        var refundTransaction = (await _transactionRepository.GetByBillwerkTransactionIdAsync(request.RefundTransactionId))
            .To<RefundTransaction>();

        if (refundTransaction.States.Current is TransactionErrorState errorState)
            return BuildPaymentErrorDto(errorState);

        return Result<RefundResponseDto, PaymentErrorDto>.Ok(refundTransaction.To(RefundTransactionToRefundResponseDtoAsync));
    }

    private static Result<RefundResponseDto, PaymentErrorDto> BuildPaymentErrorDto(TransactionErrorState errorState) => Result<RefundResponseDto, PaymentErrorDto>.Failure(new PaymentErrorDto
    {
        ErrorCode = errorState.ErrorCode,
        ErrorMessage = errorState.ErrorMessage,
        PspErrorCode = errorState.PspErrorCode,
        ReceivedAt = errorState.ReceivedAt,
    });

    private static RefundResponseDto RefundTransactionToRefundResponseDtoAsync(RefundTransaction transaction) =>
        RefundTransactionToInitialRefundResponseDto(transaction)
            .Do(responseDto => AddRefundTransactionLastStateIntoDto(transaction, responseDto));

    private static void AddRefundTransactionLastStateIntoDto(RefundTransaction transaction, RefundResponseDto refundResponseDto)
    {
        var currentState = transaction.States.Current;
        refundResponseDto.LastUpdated = currentState.LastSeenAt;
        refundResponseDto.Status = currentState.Status;
        refundResponseDto.RefundedAmount = currentState.Status == PaymentTransactionStatus.Failed 
            ? decimal.Zero
            : transaction.RequestedAmount.Value;
    }

    private static RefundResponseDto RefundTransactionToInitialRefundResponseDto(RefundTransaction refundTransaction) =>
        new()
        {
            ExternalTransactionId = refundTransaction.Id.ToString(),
            TransactionId = refundTransaction.BillwerkTransactionId.Value,
            PspTransactionId = refundTransaction.PspTransactionId,
            Currency = refundTransaction.Currency.Value,
            RequestedAmount = refundTransaction.RequestedAmount,
        };
}

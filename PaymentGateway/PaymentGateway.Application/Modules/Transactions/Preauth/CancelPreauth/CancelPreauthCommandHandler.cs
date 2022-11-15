using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Preauth;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.CancelPreauth;

public class CancelPreauthCommandHandler : IRequestHandler<CancelPreauthCommand, Result<PaymentCancellationResponseDto, PaymentErrorDto>>
{
    private readonly IPspPreauthHandler _pspPreauthHandler;
    private readonly ITransactionRepository _transactionRepository;

    public CancelPreauthCommandHandler(
        IEnumerable<Meta<Lazy<IPspPreauthHandler>, PaymentProvider>> pspPreauthHandlers,
        ITransactionRepository transactionRepository,
        PspExecutionContext pspExecutionContext)
    {
        _pspPreauthHandler = pspPreauthHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PaymentCancellationResponseDto, PaymentErrorDto>> Handle(CancelPreauthCommand request, CancellationToken cancellationToken)
    {
        var transaction = (await _transactionRepository.GetByBillwerkTransactionIdAsync(request.TransactionId)).To<PreauthTransaction>();
        var commandResult = await _pspPreauthHandler.CancelPreauthAsync(transaction.GetPspTransactionIdOrNull());

        return await commandResult
            .OnSuccess(cancellationResponseDto => AddPreauthStateAndSetIds(transaction, cancellationResponseDto))
            .OnError(paymentErrorDto => AddErrorState(transaction, paymentErrorDto))
            .FinallyAsync(_transactionRepository.UpdateAsync(transaction));
    }

    private static void AddPreauthStateAndSetIds(PreauthTransaction transaction, PaymentCancellationResponseDto cancellationData)
    {
        cancellationData.TransactionId = transaction.BillwerkTransactionId.Value;
        AddPreauthState(transaction, cancellationData);
    }

    private static void AddPreauthState(PreauthTransaction transaction, PaymentCancellationResponseDto dto)
    {
        var transactionStatus = dto.CancellationStatus switch
        {
            PaymentCancellationStatus.Succeeded => PaymentTransactionStatus.Cancelled,
            var _ => throw new ArgumentOutOfRangeException(null,
                $"Transaction {dto.TransactionId}: unexpected cancellation status {dto.CancellationStatus}"),
        };

        var currentState = transaction.States.Current.To<PreauthTransactionState>();
        var newState = new PreauthTransactionState(new(currentState.AuthorizedAmount), transactionStatus, DateTime.UtcNow);
        transaction.States.Add(newState);
    }

    private static void AddErrorState(PreauthTransaction transaction, PaymentErrorDto paymentErrorDto) =>
        transaction.States.Add(FailedStateBuilder.Build(paymentErrorDto));
}

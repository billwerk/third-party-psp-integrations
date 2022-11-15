using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Refund.SendRefund;

public class SendRefundCommandHandler : IRequestHandler<SendRefundCommand, Result<RefundResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPspRefundHandler _pspPaymentHandler;
    
    public SendRefundCommandHandler(
        IEnumerable<Meta<Lazy<IPspRefundHandler>, PaymentProvider>> pspRefundHandlers,
        ITransactionRepository transactionRepository,
        PspExecutionContext pspExecutionContext)
    {
        _transactionRepository = transactionRepository;
        _pspPaymentHandler = pspRefundHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
    }

    public async Task<Result<RefundResponseDto, PaymentErrorDto>> Handle(SendRefundCommand command, CancellationToken cancellationToken)
    {
        var paymentTransaction = (await _transactionRepository.GetByBillwerkTransactionIdAsync(command.PaymentTransactionId)).To<PaymentTransaction>();
        var refundTransaction = await CreateRefundAsync(command, paymentTransaction);
        var refundResponse = await _pspPaymentHandler.SendRefundAsync(paymentTransaction.PspTransactionId, command.InitialRefundRequestDto, paymentTransaction.PspTransactionData);
        
        return await refundResponse
            .OnSuccess(dto => AddRefundStateAndSetIds(refundTransaction, dto))
            .OnError(dto => AddErrorState(refundTransaction, dto))
            .FinallyAsync(_transactionRepository.UpdateAsync(refundTransaction))
            .MapResultAsync(x => x.Response);
    }

    private static void AddRefundStateAndSetIds(RefundTransaction transaction, ExtendedResponse<RefundResponseDto> refundResponseDto)
    {
        refundResponseDto.Response.ExternalTransactionId = transaction.Id.ToString();

        transaction.PspTransactionId = new NotEmptyString(refundResponseDto.Response.PspTransactionId);
        transaction.States.Add(new RefundTransactionState(refundResponseDto.Response.Status, DateTime.UtcNow));

        transaction.PspTransactionData = refundResponseDto.PspTransactionData;
    }

    private static void AddErrorState(RefundTransaction refundTransaction, PaymentErrorDto paymentErrorDto) =>
        refundTransaction.States.Add(new TransactionErrorState(paymentErrorDto.ReceivedAt, paymentErrorDto.ErrorMessage, paymentErrorDto.ErrorCode, paymentErrorDto.PspErrorCode));
    
    private async Task<RefundTransaction> CreateRefundAsync(SendRefundCommand command, PaymentTransaction paymentTransaction)
    {
        var refundTransaction = new RefundTransaction(command.Amount,
            command.Currency,
            command.TransactionId,
            command.PaymentTransactionId,
            command.WebhookTarget,
            paymentTransaction.PaymentMethodInfo,
            command.PspSettingsId);

        return (await _transactionRepository.InsertAsync(refundTransaction)).To<RefundTransaction>();
    }
}

using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses.Payment;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Payment.FetchPayment;

public class FetchPaymentCommandHandler : IRequestHandler<FetchPaymentCommand, Result<PaymentResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAgreementRepository _agreementRepository;

    public FetchPaymentCommandHandler(
        ITransactionRepository transactionRepository,
        IAgreementRepository agreementRepository)
    {
        _transactionRepository = transactionRepository;
        _agreementRepository = agreementRepository;
    }

    public async Task<Result<PaymentResponseDto, PaymentErrorDto>> Handle(FetchPaymentCommand command, CancellationToken cancellationToken)
    {
        var transaction = (await _transactionRepository.GetByBillwerkTransactionIdAsync(command.PaymentTransactionId)).To<PaymentTransaction>();

        if (transaction.States.Current is TransactionErrorState errorState)
            return BuildPaymentErrorDto(errorState);

        var responseDto = await TransactionToPaymentResponseDtoAsync(transaction);

        return Result<PaymentResponseDto, PaymentErrorDto>.Ok(responseDto);
    }

    private static Result<PaymentResponseDto, PaymentErrorDto> BuildPaymentErrorDto(TransactionErrorState errorState) => Result<PaymentResponseDto, PaymentErrorDto>.Failure(new PaymentErrorDto
    {
        ErrorCode = errorState.ErrorCode,
        ErrorMessage = errorState.ErrorMessage,
        PspErrorCode = errorState.PspErrorCode!,
        ReceivedAt = errorState.ReceivedAt,
    });

    private async Task<PaymentResponseDto> TransactionToPaymentResponseDtoAsync(PaymentTransaction transaction) =>
        (await TransactionToInitialPaymentResponseDtoAsync(transaction)).Do(responseDto => AddTransactionCurrentStateIntoDto(transaction, responseDto));

    private static void AddTransactionCurrentStateIntoDto(PaymentTransaction transaction, PaymentResponseDto paymentResponseDto)
    {
        var currentState = (PaymentTransactionState)transaction.States.Current;
        paymentResponseDto.LastUpdated = currentState.LastSeenAt;
        paymentResponseDto.Status = currentState.Status;
        
        paymentResponseDto.Payments = PaymentItemDtosFromState(currentState);
        paymentResponseDto.Chargebacks = ChargebackItemDtosState(currentState);
        paymentResponseDto.RefundedAmount = currentState.RefundedAmount;
        paymentResponseDto.RefundableAmount = currentState.PaidAmount - currentState.RefundedAmount;
    }

    private async Task<PaymentResponseDto> TransactionToInitialPaymentResponseDtoAsync(PaymentTransaction transaction) =>
        (await _agreementRepository.GetAgreementByIdAsync(transaction.AgreementId)).To(agreement =>
            new PaymentResponseDto
            {
                ExternalTransactionId = transaction.Id.ToString(),
                TransactionId = transaction.BillwerkTransactionId.Value,
                PspTransactionId = transaction.PspTransactionId,
                Bearer = agreement.PaymentBearer,
                Currency = transaction.Currency.Value,
                RequestedAmount = transaction.RequestedAmount,
                DueDate = transaction.DueDate,
            });

    private static List<PaymentItemDto> PaymentItemDtosFromState(PaymentTransactionState state) =>
        state.Payments.Select(item => new PaymentItemDto
        {
            ExternalItemId = item.PspItemId,
            Description = item.Description,
            BookingDate = item.BookingDate,
            Amount = item.PositiveAmount,
        }).ToList();

    private static List<PaymentChargebackItemDto> ChargebackItemDtosState(PaymentTransactionState state) =>
        state.Chargebacks.OrEmpty().Select(itemDto => new PaymentChargebackItemDto
        {
            ExternalItemId = itemDto.PspItemId,
            Description = itemDto.Description,
            BookingDate = itemDto.BookingDate,
            Amount = itemDto.Amount,
            Reason = itemDto.Reason,
            FeeAmount = itemDto.FeeAmount,
            PspReasonCode = itemDto.PspReasonCode,
            PspReasonMessage = itemDto.PspReasonMessage,
        }).ToList();
}

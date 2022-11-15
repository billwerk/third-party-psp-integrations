using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Modules.Transactions.Payment.SendPayment;

public class SendPaymentCommandHandler : IRequestHandler<SendPaymentCommand, Result<PaymentResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAgreementRepository _agreementRepository;
    private readonly IPspPaymentHandler _pspPaymentHandler;

    public SendPaymentCommandHandler(IEnumerable<Meta<Lazy<IPspPaymentHandler>, PaymentProvider>> pspPaymentHandlers,
        ITransactionRepository transactionRepository,
        IAgreementRepository agreementRepository,
        PspExecutionContext pspExecutionContext)
    {
        _transactionRepository = transactionRepository;
        _agreementRepository = agreementRepository;
        _pspPaymentHandler = pspPaymentHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
    }

    public async Task<Result<PaymentResponseDto, PaymentErrorDto>> Handle(SendPaymentCommand command, CancellationToken cancellationToken)
    {
        var transaction = await CreateTransactionAsync(command);
        var agreement = await _agreementRepository.GetAgreementByIdAsync(command.AgreementId);
        var commandResult = await _pspPaymentHandler.SendPaymentAsync(command.InitialPaymentRequestDto, agreement);

        return await commandResult
            .OnSuccess(responseDto => AddPaymentStateAndSetInfoFromPsp(transaction, responseDto, command.InitialPaymentRequestDto))
            .OnError(errorDto => AddErrorState(transaction, errorDto))
            .FinallyAsync(_transactionRepository.UpdateAsync(transaction))
            .MapResultAsync(x => x.Response);
    }

    private static void AddPaymentStateAndSetInfoFromPsp(PaymentTransaction transaction, ExtendedResponse<PaymentResponseDto> responseDto, PaymentRequestDto requestDto)
    {
        responseDto.Response.ExternalTransactionId = transaction.Id.ToString();
        responseDto.Response.TransactionId = requestDto.TransactionId;

        transaction.PspTransactionId = new NotEmptyString(responseDto.Response.PspTransactionId);
        transaction.States.Add(ResponseDtoToPaymentState(responseDto.Response));
        transaction.DueDate = responseDto.Response.DueDate;

        transaction.PspTransactionData = responseDto.PspTransactionData;
    }

    private static PaymentTransactionState ResponseDtoToPaymentState(PaymentResponseDto responseDto) =>
        new(responseDto.Status, responseDto.LastUpdated, PaymentItemsFromResponseDto(responseDto),
            Array.Empty<RefundItem>(), Array.Empty<ChargebackItem>());

    private static IReadOnlyCollection<PaymentItem> PaymentItemsFromResponseDto(PaymentResponseDto responseDto) =>
        responseDto.Payments.Select(itemDto => new PaymentItem
        {
            PspItemId = itemDto.ExternalItemId,
            Description = itemDto.Description,
            BookingDate = itemDto.BookingDate,
            PositiveAmount = new PositiveAmount(itemDto.Amount),
        }).ToList();

    private static void AddErrorState(PaymentTransaction transaction, PaymentErrorDto paymentErrorDto) =>
        transaction.States.Add(new TransactionErrorState(paymentErrorDto.ReceivedAt, paymentErrorDto.ErrorMessage, paymentErrorDto.ErrorCode, paymentErrorDto.PspErrorCode));

    private async Task<PaymentTransaction> CreateTransactionAsync(SendPaymentCommand command) =>
        await new PaymentTransaction(
            command.RequestedPositiveAmount,
            command.Currency,
            command.TransactionId,
            command.WebhookTarget,
            command.PaymentMethodInfo,
            command.AgreementId,
            command.PspSettingsId,
            new(command.TransactionReferenceText))
        .To(async transaction => (await _transactionRepository.InsertAsync(transaction)).To<PaymentTransaction>());
}

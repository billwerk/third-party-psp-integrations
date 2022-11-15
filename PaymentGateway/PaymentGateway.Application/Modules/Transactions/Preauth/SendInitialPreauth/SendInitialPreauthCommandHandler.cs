using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using MediatR;
using PaymentGateway.Shared;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Preauth;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.SendInitialPreauth;

public class SendInitialPreauthCommandHandler : IRequestHandler<SendInitialPreauthCommand, Result<PreauthResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPspPreauthHandler _pspPreauthHandler;

    public SendInitialPreauthCommandHandler(
        IEnumerable<Meta<Lazy<IPspPreauthHandler>, PaymentProvider>> pspPreauthHandlers,
        ITransactionRepository transactionRepository,
        PspExecutionContext pspExecutionContext)
    {
        _pspPreauthHandler = pspPreauthHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PreauthResponseDto, PaymentErrorDto>> Handle(SendInitialPreauthCommand command, CancellationToken cancellationToken)
    {
        var preauthTransaction = await CreateTransactionAsync(command);
        var commandResult = await _pspPreauthHandler.SendInitialPreauthAsync(command.InitialPreauthRequestDto);

        return await commandResult
            .OnSuccess(initialResponse => AddPreauthStateAndSetExternalIdToDto(preauthTransaction, initialResponse))
            .OnError(paymentErrorDto => AddErrorState(preauthTransaction, paymentErrorDto))
            .Map(initialResponse => initialResponse.PreauthResponseDto)
            .FinallyAsync(_transactionRepository.UpdateAsync(preauthTransaction));
    }

    private static void AddPreauthStateAndSetExternalIdToDto(PreauthTransaction transaction, InitialResponse initialResponse)
    {
        initialResponse.PreauthResponseDto.ExternalTransactionId = transaction.Id.ToString();
        initialResponse.PreauthResponseDto.TransactionId = transaction.BillwerkTransactionId.Value;

        //For Redirect flow it's possible to not have PspTransactionId and Bearer after the Initial preauth request
        //In the Redirect flow case PspTransactionId and Bearer will be filled after the finalize call.
        if (!string.IsNullOrWhiteSpace(initialResponse.PreauthResponseDto.PspTransactionId))
            transaction.PspTransactionId = new(initialResponse.PreauthResponseDto.PspTransactionId);

        UpdateAgreement(transaction, initialResponse);
        AddPreauthState(transaction, initialResponse.PreauthResponseDto);
    }

    private static void UpdateAgreement(PreauthTransaction transaction, InitialResponse initialResponse)
    {
        if (initialResponse.PreauthResponseDto.Bearer.OrEmpty().Any())
            transaction.Agreement.PaymentBearer = initialResponse.PreauthResponseDto.Bearer;

        if (!string.IsNullOrWhiteSpace(initialResponse.PspAgreementId))
            transaction.Agreement.PspAgreementId = initialResponse.PspAgreementId;
    }

    private static void AddPreauthState(PreauthTransaction transaction, PreauthResponseDto responseDto) =>
        transaction.States.Add(new PreauthTransactionState(new(responseDto.AuthorizedAmount), responseDto.Status, DateTime.UtcNow));

    private static void AddErrorState(PreauthTransaction transaction, PaymentErrorDto paymentErrorDto) =>
        transaction.States.Add(FailedStateBuilder.Build(paymentErrorDto));

    private async Task<PreauthTransaction> CreateTransactionAsync(SendInitialPreauthCommand command) =>
        await new PreauthTransaction(command.InitialAgreement,
                command.RequestedNonNegativeAmount,
                command.Currency,
                command.TransactionId,
                command.PaymentMethodInfo,
                command.WebhookTarget,
                command.PspSettingsId,
                command.InitialPreauthRequestDto.IsInitial)
            .To(async preauthTransaction => (await _transactionRepository.InsertAsync(preauthTransaction)).To<PreauthTransaction>());
}

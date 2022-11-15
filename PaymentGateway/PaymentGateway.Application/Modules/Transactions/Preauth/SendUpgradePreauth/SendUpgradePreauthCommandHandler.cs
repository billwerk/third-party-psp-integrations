using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.SendUpgradePreauth;

public class SendUpgradePreauthCommandHandler : IRequestHandler<SendUpgradePreauthCommand, Result<PreauthResponseDto, PaymentErrorDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAgreementRepository _agreementRepository;
    private readonly IPspPreauthHandler _pspPreauthHandler;

    public SendUpgradePreauthCommandHandler(IEnumerable<Meta<Lazy<IPspPreauthHandler>, PaymentProvider>> pspPreauthHandlers,
        ITransactionRepository transactionRepository,
        IAgreementRepository agreementRepository,
        PspExecutionContext pspExecutionContext)
    {
        _pspPreauthHandler = pspPreauthHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
        _transactionRepository = transactionRepository;
        _agreementRepository = agreementRepository;
    }

    public async Task<Result<PreauthResponseDto, PaymentErrorDto>> Handle(SendUpgradePreauthCommand command, CancellationToken cancellationToken)
    {
        var preauthTransaction = await CreateTransactionAsync(command, command.InitialAgreement);
        var agreement = await _agreementRepository.GetAgreementByIdAsync(command.AgreementId);
        var commandResult = await _pspPreauthHandler.SendUpgradePreauthAsync(command.InitialPreauthRequestDto, agreement);

        return await commandResult
            .OnSuccess(preauthResponseDto => AddPreauthStateAndSetExternalIdToDto(preauthTransaction, preauthResponseDto))
            .OnError(paymentErrorDto => AddErrorState(preauthTransaction, paymentErrorDto))
            .Map(response => response.PreauthResponseDto)
            .FinallyAsync(_transactionRepository.UpdateAsync(preauthTransaction));
    }

    private static void AddPreauthStateAndSetExternalIdToDto(PreauthTransaction transaction, InitialResponse initialResponse)
    {
        initialResponse.PreauthResponseDto.ExternalTransactionId = transaction.Id.ToString();
        initialResponse.PreauthResponseDto.TransactionId = transaction.BillwerkTransactionId.Value;

        if (!string.IsNullOrWhiteSpace(initialResponse.PspAgreementId))
            transaction.Agreement.PspAgreementId = initialResponse.PspAgreementId;

        transaction.PspTransactionId = new NotEmptyString(initialResponse.PreauthResponseDto.PspTransactionId);
        transaction.Agreement.PaymentBearer = initialResponse.PreauthResponseDto.Bearer;
        transaction.States.Add(new PreauthTransactionState(
            new NonNegativeAmount(initialResponse.PreauthResponseDto.RequestedAmount),
            initialResponse.PreauthResponseDto.Status,
            DateTime.UtcNow));

        transaction.PspTransactionData = initialResponse.PspTransactionData;
    }

    private static void AddErrorState(PreauthTransaction preauthTransaction, PaymentErrorDto paymentErrorDto) =>
        preauthTransaction.States.Add(new TransactionErrorState(paymentErrorDto.ReceivedAt, paymentErrorDto.ErrorMessage, paymentErrorDto.ErrorCode, paymentErrorDto.PspErrorCode));

    private async Task<PreauthTransaction> CreateTransactionAsync(SendUpgradePreauthCommand command, Domain.Modules.Transactions.Agreement.Agreement initialAgreement) =>
        await new PreauthTransaction(initialAgreement,
                command.RequestedNonNegativeAmount,
                command.Currency,
                command.TransactionId,
                command.PaymentMethodInfo,
                command.WebhookTarget,
                command.PspSettingsId,
                command.InitialPreauthRequestDto.IsInitial)
            .To(async preauthTransaction => (await _transactionRepository.InsertAsync(preauthTransaction)).To<PreauthTransaction>());
}

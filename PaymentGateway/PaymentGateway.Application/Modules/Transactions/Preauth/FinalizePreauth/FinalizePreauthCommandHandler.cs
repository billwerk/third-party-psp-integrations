// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.FinalizePreauth;

public class FinalizePreauthCommandHandler : IRequestHandler<FinalizePreauthCommand, Result<PreauthResponseDto, PaymentErrorDto>>
{
    private readonly ISupportFinalizePreauth _pspFinalizeHandler;
    private readonly ITransactionRepository _transactionRepository;

    public FinalizePreauthCommandHandler(
        IEnumerable<Meta<Lazy<ISupportFinalizePreauth>, PaymentProvider>> pspPreauthHandlers,
        ITransactionRepository transactionRepository,
        PspExecutionContext pspExecutionContext)
    {
        _pspFinalizeHandler = pspPreauthHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PreauthResponseDto, PaymentErrorDto>> Handle(FinalizePreauthCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByBillwerkTransactionIdAsync(request.PreauthTransactionId);

        var finalizationData = EnrichFinalizationData(request.FinalizationData.ToDictionary(item => item.Key, item => (object)item.Value), transaction);
        var commandResult = await _pspFinalizeHandler.FinalizePreauthAsync(GetPspTransactionIdOrNull(transaction), finalizationData);

        return await commandResult
            .OnSuccess(initialResponse => AddTransactionStateAndSetDataFromPsp(transaction, initialResponse))
            .OnError(paymentErrorDto => AddErrorState(transaction, paymentErrorDto))
            .Map(response => response.PreauthResponseDto)
            .FinallyAsync(_transactionRepository.UpdateAsync(transaction));
    }

    private static void AddErrorState(Transaction preauthTransaction, PaymentErrorDto paymentErrorDto) =>
        preauthTransaction.States.Add(FailedStateBuilder.Build(paymentErrorDto));

    private static void AddTransactionStateAndSetDataFromPsp(Transaction transaction, InitialResponse initialResponse)
        => transaction.Do(initialResponse, AddDataToTransactionAndDto).Do(initialResponse, AddTransactionState);

    private static void AddDataToTransactionAndDto(Transaction transaction, InitialResponse initialResponse)
    {
        //For trial case it's possible to not have a PspTransactionId
        if (!string.IsNullOrWhiteSpace(initialResponse.PreauthResponseDto.PspTransactionId))
            transaction.PspTransactionId = new NotEmptyString(initialResponse.PreauthResponseDto.PspTransactionId);

        var withAgreement = transaction.To<PreauthTransaction>();
        if (!string.IsNullOrWhiteSpace(initialResponse.PspAgreementId))
            withAgreement.Agreement.PspAgreementId = initialResponse.PspAgreementId;

        withAgreement.Agreement.PaymentBearer = initialResponse.PreauthResponseDto.Bearer;
        initialResponse.PreauthResponseDto.ExternalTransactionId = transaction.Id.ToString();
        transaction.PspTransactionData = initialResponse.PspTransactionData;
    }

    private static void AddTransactionState(Transaction transaction, InitialResponse response)
    {
        var responseDto = response.PreauthResponseDto;
        var state = new PreauthTransactionState(new NonNegativeAmount(responseDto.AuthorizedAmount), responseDto.Status, DateTime.UtcNow);
        transaction.States.Add(state);
    }

    private static IDictionary<string, object> EnrichFinalizationData(IDictionary<string, object> initialFinalizationData, Transaction transaction) => new Dictionary<string, object>(initialFinalizationData)
    {
        [nameof(PreauthRequestDto.Currency)] = transaction.Currency,
        [nameof(PreauthRequestDto.RequestedAmount)] = transaction switch
        {
            PreauthTransaction preauth => preauth.RequestedAmount,
            _ => throw new ArgumentOutOfRangeException(nameof(transaction), transaction, null),
        },
    };

    private static NotEmptyString? GetPspTransactionIdOrNull(Transaction transaction) =>
        transaction.To<PreauthTransaction>().GetPspTransactionIdOrNull();
}

// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Shared;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.CapturePreauth;

public class CapturePreauthCommandHandler : IRequestHandler<CapturePreauthCommand, Result<PaymentResponseDto, PaymentErrorDto>>
{
    private readonly ISupportCapturePreauth _pspPaymentHandler;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAgreementRepository _agreementRepository;

    public CapturePreauthCommandHandler(
        IEnumerable<Meta<Lazy<ISupportCapturePreauth>, PaymentProvider>> pspPaymentHandlers,
        ITransactionRepository transactionRepository,
        IAgreementRepository agreementRepository,
        PspExecutionContext pspExecutionContext)
    {
        _transactionRepository = transactionRepository;
        _agreementRepository = agreementRepository;
        _pspPaymentHandler = pspPaymentHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
    }

    public async Task<Result<PaymentResponseDto, PaymentErrorDto>> Handle(CapturePreauthCommand command, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByBillwerkTransactionIdAsync(command.PreauthTransactionId!);
        var payment = await CreateOrFindRelatedPaymentAsync(transaction, command);
        var agreement = await _agreementRepository.GetAgreementByIdAsync(command.AgreementId);
        var commandResult = await _pspPaymentHandler.CapturePreauthAsync(command.InitialPaymentRequestDto, agreement, transaction.PspTransactionId);

        return await commandResult
            .OnSuccess(responseDto => AddPaymentStateAndSetInfoFromPsp(payment, responseDto, command.InitialPaymentRequestDto))
            .OnError(errorDto => AddErrorState(payment, errorDto))
            .FinallyAsync(_transactionRepository.UpdateAsync(payment))
            .MapResultAsync(x => x.Response);
    }

    private static void AddPaymentStateAndSetInfoFromPsp(Transaction transaction, ExtendedResponse<PaymentResponseDto> responseDto, PaymentRequestDto requestDto)
    {
        responseDto.Response.ExternalTransactionId = transaction.Id.ToString();
        responseDto.Response.TransactionId = requestDto.TransactionId;

        transaction.PspTransactionId = new NotEmptyString(responseDto.Response.PspTransactionId);
        transaction.States.Add(ResponseDtoToPaymentState(responseDto.Response));

        transaction.PspTransactionData = responseDto.PspTransactionData;
    }

    private static PaymentTransactionState ResponseDtoToPaymentState(PaymentResponseDto responseDto) =>
        new(responseDto.Status,
            responseDto.LastUpdated,
            PaymentItemsFromResponseDto(responseDto),
            Array.Empty<RefundItem>(),
            Array.Empty<ChargebackItem>());

    private static IReadOnlyCollection<PaymentItem> PaymentItemsFromResponseDto(PaymentResponseDto responseDto) =>
        responseDto.Payments.Select(itemDto => new PaymentItem
            {
                PspItemId = itemDto.ExternalItemId,
                Description = itemDto.Description,
                BookingDate = itemDto.BookingDate,
                PositiveAmount = new PositiveAmount(itemDto.Amount),
            })
            .ToList();

    private static void AddErrorState(Transaction transaction, PaymentErrorDto paymentErrorDto) =>
        transaction.States.Add(new TransactionErrorState(paymentErrorDto.ReceivedAt, paymentErrorDto.ErrorMessage, paymentErrorDto.ErrorCode, paymentErrorDto.PspErrorCode));

    private async Task<PaymentTransaction> CreateOrFindRelatedPaymentAsync(Transaction transaction, CapturePreauthCommand command)
        => await new PaymentTransaction(command.RequestedPositiveAmount,
                command.Currency,
                command.TransactionId,
                command.WebhookTarget,
                command.PaymentMethodInfo,
                command.AgreementId,
                command.PspSettingsId,
                new(command.TransactionReferenceText))
            .To(async payment => (await _transactionRepository.InsertAsync(payment)).To<PaymentTransaction>());
}

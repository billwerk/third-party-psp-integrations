// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace FakeProvider.Handlers;

public class FakeProviderPreauthHandler : FakeProviderHandlerBase, IPspPreauthHandler, ISupportCapturePreauth
{
    public Task<Result<InitialResponse, PaymentErrorDto>> SendInitialPreauthAsync(PreauthRequestDto preauthRequestDto)
    {
        var resultOfFakePspCall = preauthRequestDto.PaymentMeansReference.Role switch
        {
            PublicPaymentProviderRole.Debit => FakeProviderWrapper.SendDebitInitialPreauth(preauthRequestDto),
            PublicPaymentProviderRole.CreditCard => FakeProviderWrapper.SendCreditCardInitialPreauth(preauthRequestDto),
            _ => throw new ArgumentOutOfRangeException("Not supported payment role by FakeProvider: " +
                                                       $"{preauthRequestDto.PaymentMeansReference.Role}. Supported roles:" +
                                                       $"{PublicPaymentProviderRole.CreditCard} and {PublicPaymentProviderRole.Debit}"),
        };

        return resultOfFakePspCall
            .BiMap(CreateInitialResponse, error => error)
            .To(Task.FromResult);
    }

    public Task<Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto>> CapturePreauthAsync(PaymentRequestDto paymentRequestDto, Agreement agreement, NotEmptyString preauthPspTransactionId)
    {
        var resultOfFakePspCall = paymentRequestDto.PaymentMeansReference.Role switch
        {
            PublicPaymentProviderRole.CreditCard => FakeProviderWrapper.CapturePreauthTransaction(paymentRequestDto,
                agreement,
                preauthPspTransactionId),
            PublicPaymentProviderRole.Debit => new PaymentErrorDto
            {
                ErrorCode = PaymentErrorCode.UnmappedError,
                ErrorMessage = "Direct debit can not process capture transactions cause nothing to capture."
            },
            _ => throw new ArgumentOutOfRangeException("Not supported payment role by FakeProvider: " +
                                                       $"{paymentRequestDto.PaymentMeansReference.Role}. Supported roles:" +
                                                       $"{PublicPaymentProviderRole.CreditCard} and {PublicPaymentProviderRole.Debit}"),
        };

        return resultOfFakePspCall
            .BiMap(CreateExtendedResponse, error => error)
            .To(Task.FromResult);
    }

    //Just for simplifying no fake provider wrapper calls for SendUpgradePreauthAsync / CancelPreauthAsync.
    public Task<Result<InitialResponse, PaymentErrorDto>> SendUpgradePreauthAsync(PreauthRequestDto preauthRequestDto, Agreement agreement)
        => new PreauthResponseDto
            {
                AuthorizedAmount = preauthRequestDto.RequestedAmount,
                Bearer = agreement.PaymentBearer,
                Currency = preauthRequestDto.Currency,
                PspTransactionId = Guid.NewGuid().ToString(),
                RequestedAmount = preauthRequestDto.RequestedAmount,
                ExpiresAt = DateTimeOffset.Now.AddDays(7),
            }.To(dto => new InitialResponse(dto, null))
            .To(Result<InitialResponse, PaymentErrorDto>.Ok)
            .To(Task.FromResult);

    public Task<Result<PaymentCancellationResponseDto, PaymentErrorDto>> CancelPreauthAsync(NotEmptyString? pspTransactionId)
        => new PaymentCancellationResponseDto
            {
                CancellationStatus = PaymentCancellationStatus.Succeeded,
            }.To(Result<PaymentCancellationResponseDto, PaymentErrorDto>.Ok)
            .To(Task.FromResult);
}

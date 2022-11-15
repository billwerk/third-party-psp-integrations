// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Shared;

namespace FakeProvider.Handlers;

public class FakeProviderPaymentHandler : FakeProviderHandlerBase, IPspPaymentHandler
{
    public Task<Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto>> SendPaymentAsync(PaymentRequestDto paymentRequestDto, Agreement agreement)
    {
        var resultOfFakePspCall = paymentRequestDto.PaymentMeansReference.Role switch
        {
            PublicPaymentProviderRole.Debit => FakeProviderWrapper.SendDebitPayment(paymentRequestDto, agreement),
            PublicPaymentProviderRole.CreditCard => FakeProviderWrapper.SendCreditPayment(paymentRequestDto, agreement),
            _ => throw new ArgumentOutOfRangeException("Not supported payment role by FakeProvider: " +
                                                       $"{paymentRequestDto.PaymentMeansReference.Role}. Supported roles:" +
                                                       $"{PublicPaymentProviderRole.CreditCard} and {PublicPaymentProviderRole.Debit}"),
        };

        return resultOfFakePspCall
            .BiMap(CreateExtendedResponse, error => error)
            .To(Task.FromResult);
    }
}

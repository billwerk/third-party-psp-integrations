// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using FakeProvider.Bearers;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace FakeProvider.Wrapper;

public class FakeProviderWrapper
{
    public Result<PreauthResponseDto, PaymentErrorDto> SendDebitInitialPreauth(PreauthRequestDto preauthRequestDto)
    {
        // Usually, for Direct Debit payment method there placed calls to create mandate/customer/agreement etc.. on PSP side.
        // Successfully created object on PSP side means success preauth transaction

        var paymentBearer = preauthRequestDto.PaymentMeansReference.InitialBearer.ConvertToObject<DirectDebitSimpleBearer>();

        //Simulation of checking bearer, can be responsibility of PSP, placed here as sample of payment method
        //which support initial payment bearer.

        if (!paymentBearer.IsValid())
            return Result<PreauthResponseDto, PaymentErrorDto>.Failure(new PaymentErrorDto
            {
                ErrorCode = PaymentErrorCode.BearerInvalid,
                ErrorMessage = "Invalid initial bearer for direct debit payment method in FakeProvider.",
                ReceivedAt = DateTime.UtcNow,
            });

        var resultOfCall = SimulateFakeProviderCall(preauthRequestDto, preauthRequestDto.RequestedAmount);

        if (resultOfCall.IsFailure)
            return resultOfCall.Error;

        paymentBearer.MandateId = resultOfCall.Data.ToString();

        return new PreauthResponseDto
        {
            AuthorizedAmount = Decimal.Zero,
            Bearer = paymentBearer.ToDictionaryFromPropertiesWithSetter(),
            Currency = preauthRequestDto.Currency,
            // Id of created object on PSP side is a good candidate for PspTransactionId value, even if it is not a "transaction",
            // since it is anyway an object which created in result of preauth call and confirm preauth process.
            PspTransactionId = paymentBearer.MandateId,
            RequestedAmount = preauthRequestDto.RequestedAmount,
            Status = PaymentTransactionStatus.Succeeded,
            LastUpdated = DateTime.UtcNow,
        }.To(Result<PreauthResponseDto, PaymentErrorDto>.Ok);
    }

    public Result<PreauthResponseDto, PaymentErrorDto> SendCreditCardInitialPreauth(PreauthRequestDto preauthRequestDto)
    {
        //Simulate call to Psp which return a credit card bearer.
        var creditCardPaymentBearer = new CreditCardSimpleBearer
        {
            CardType = "Mastercard",
            ExpiryMonth = 1,
            ExpiryYear = 2025,
            Holder = "JOHN DOE",
            Country = "Germany",
            Last4CardNumber = "**1234",
        };

        var resultOfCall = SimulateFakeProviderCall(preauthRequestDto, preauthRequestDto.RequestedAmount);

        if (resultOfCall.IsFailure)
            return resultOfCall.Error;

        return new PreauthResponseDto
        {
            AuthorizedAmount = Decimal.Zero,
            Bearer = creditCardPaymentBearer.ToDictionaryFromPropertiesWithSetter(),
            Currency = preauthRequestDto.Currency,
            PspTransactionId = resultOfCall.Data.ToString(),
            RequestedAmount = preauthRequestDto.RequestedAmount,
            Status = PaymentTransactionStatus.Succeeded,
            LastUpdated = DateTime.UtcNow,
        };
    }

    public Result<PaymentResponseDto, PaymentErrorDto> CapturePreauthTransaction(
        PaymentRequestDto paymentRequestDto,
        Agreement agreement,
        NotEmptyString preauthPspTransactionId)
    {
        var resultOfCall = SimulateFakeProviderCall(paymentRequestDto, paymentRequestDto.RequestedAmount);
        
        if (resultOfCall.IsFailure)
            return resultOfCall.Error;

        return new PaymentResponseDto
        {
            Currency = paymentRequestDto.Currency,
            Payments = new List<PaymentItemDto>
            {
                new()
                {
                    Amount = paymentRequestDto.RequestedAmount,
                    BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
                    Description = "Successful full payment",
                    ExternalItemId = Guid.NewGuid().ToString(),
                },
            },
            LastUpdated = DateTime.UtcNow,
            PspTransactionId = Guid.NewGuid().ToString(),
            RefundableAmount = paymentRequestDto.RequestedAmount,
            RefundedAmount = Decimal.Zero,
            Bearer = new Dictionary<string, string>
            {
                { "PaymentRoleSpecificToken", Guid.NewGuid().ToString() },
            },
            Status = PaymentTransactionStatus.Succeeded,
        }.To(Result<PaymentResponseDto, PaymentErrorDto>.Ok);
    }

    public Result<PaymentResponseDto, PaymentErrorDto> SendDebitPayment(
        PaymentRequestDto paymentRequestDto,
        Agreement agreement)
    {
        var resultOfCall = SimulateFakeProviderCall(paymentRequestDto, paymentRequestDto.RequestedAmount);
        
        if (resultOfCall.IsFailure)
            return resultOfCall.Error;
        
        return new PaymentResponseDto
            {
                Currency = paymentRequestDto.Currency,
                Payments = new List<PaymentItemDto>
                {
                    new()
                    {
                        Amount = paymentRequestDto.RequestedAmount,
                        BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
                        Description = "Successful full payment",
                        ExternalItemId = Guid.NewGuid().ToString(),
                    },
                },
                LastUpdated = DateTime.UtcNow,
                PspTransactionId = Guid.NewGuid().ToString(),
                RefundableAmount = paymentRequestDto.RequestedAmount,
                RefundedAmount = decimal.Zero,
                Bearer = agreement.PaymentBearer,
                Status = PaymentTransactionStatus.Pending,
            }.To(Result<PaymentResponseDto, PaymentErrorDto>.Ok);
    }

    public Result<PaymentResponseDto, PaymentErrorDto> SendCreditPayment(
        PaymentRequestDto paymentRequestDto,
        Agreement agreement)
    {
        var resultOfCall = SimulateFakeProviderCall(paymentRequestDto, paymentRequestDto.RequestedAmount);
        
        if (resultOfCall.IsFailure)
            return resultOfCall.Error;
        
        return new PaymentResponseDto
        {
            Currency = paymentRequestDto.Currency,
            Payments = new List<PaymentItemDto>
            {
                new()
                {
                    Amount = paymentRequestDto.RequestedAmount,
                    BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
                    Description = "Successful full payment",
                    ExternalItemId = Guid.NewGuid().ToString(),
                },
            },
            LastUpdated = DateTime.UtcNow,
            PspTransactionId = Guid.NewGuid().ToString(),
            RefundableAmount = paymentRequestDto.RequestedAmount,
            RefundedAmount = decimal.Zero,
            Bearer = agreement.PaymentBearer,
            Status = PaymentTransactionStatus.Pending,
        }.To(Result<PaymentResponseDto, PaymentErrorDto>.Ok);
    }

    public Result<RefundResponseDto, PaymentErrorDto> SendRefund(RefundRequestDto refundRequestDto)
    {
        var resultOfCall = SimulateFakeProviderCall(refundRequestDto, refundRequestDto.RequestedAmount);

        if (resultOfCall.IsFailure)
            return resultOfCall.Error;

        return new RefundResponseDto
        {
            Currency = refundRequestDto.Currency,
            RefundedAmount = refundRequestDto.RequestedAmount,
            RequestedAmount = refundRequestDto.RequestedAmount,
            Description = "Successful refund",
            Status = PaymentTransactionStatus.Succeeded,
            LastUpdated = DateTime.UtcNow,
            BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
            PspTransactionId = Guid.NewGuid().ToString(),
        }.To(Result<RefundResponseDto, PaymentErrorDto>.Ok);
    }
    
    #region  PSP-call simulation methods

    private Result<Guid, PaymentErrorDto> SimulateFakeProviderCall<T>(T request, decimal requestedAmount)
        where T : IRequestDto
    {
        var resultOfFakePaymentCall = BuildFakePaymentErrorIfNeeded(requestedAmount);

        return resultOfFakePaymentCall.IsSuccess
            ? Result<Guid, PaymentErrorDto>.Ok(Guid.NewGuid())
            : resultOfFakePaymentCall.Error;
    }

    private VerificationResult<PaymentErrorDto> BuildFakePaymentErrorIfNeeded(decimal requestedAmount)
    {
        if (requestedAmount != FakeProviderConstants.TransactionAmountForErrorSimulation)
            return VerificationResult<PaymentErrorDto>.Ok();
        
        return VerificationResult<PaymentErrorDto>.Failure(new PaymentErrorDto
        {
            ErrorCode = PaymentErrorCode.InsufficientBalance,
            ErrorMessage = "Simulated payment error.",
            ReceivedAt = DateTime.UtcNow,
        });
    }

    #endregion
}

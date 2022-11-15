// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using FakeProvider.Wrapper;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules;

namespace FakeProvider.Handlers;

public abstract class FakeProviderHandlerBase : IPspHandler
{
    public PaymentProvider PaymentProvider => PaymentProvider.FakeProvider;

    protected readonly FakeProviderWrapper FakeProviderWrapper;

    protected FakeProviderHandlerBase() => FakeProviderWrapper = new FakeProviderWrapper();

    protected InitialResponse CreateInitialResponse(PreauthResponseDto preauthResponseDto)
        => new(preauthResponseDto, null);

    protected ExtendedResponse<T> CreateExtendedResponse<T>(T transactionResponseDto)
        where T : ITransactionResponseDto
        => new(transactionResponseDto);
}

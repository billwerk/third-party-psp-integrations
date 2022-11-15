// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Preauth;

public class CancelPreauthRequestExample : IExamplesProvider<PaymentCancellationRequestDto>
{
    public PaymentCancellationRequestDto GetExamples() => new() { PspSettingsId = "636a915b7267328bc225c566" };
}

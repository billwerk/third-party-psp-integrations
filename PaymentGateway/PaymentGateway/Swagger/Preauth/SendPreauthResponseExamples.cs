// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Preauth;

public class SendPreauthOrFetchCurrentStateResponseExample : IExamplesProvider<PreauthResponseDto>
{
    public PreauthResponseDto GetExamples() => new()
    {
        TransactionId = "20221118-636bd596c10cfc318b1bfabb",
        ExternalTransactionId = "636cfcfc7d50dda9cfb6a010",
        PspTransactionId = "b89dfe36-2221-49fb-8dab-df26486e1b62",
        Currency = "EUR",
        RequestedAmount = 300m,
        LastUpdated = DateTime.UtcNow,
        Status = PaymentTransactionStatus.Succeeded,
        Bearer = new Dictionary<string, string>
        {
            {"SampleCustomerPaymentData","SampleCustomerPaymentData" },
        },
    };
}

// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Refund;

public class RefundRequestExample : IExamplesProvider<RefundRequestDto>
{
    public RefundRequestDto GetExamples() => new()
    {
        PspSettingsId = "636a915b7267328bc225c566",
        TransactionId =  "20221108-636a92f57267328bc225c62a",
        RequestedAmount = 15m,
        Currency = "EUR",
        WebhookTarget = "https://sample.billwerk.com/PSPWebhooks/ExternalIntegration",
        PaymentTransactionId = "20221108-636a91f61c48d9fa186166f8",
        PayerData = new PayerDataDto
        {
            FirstName = "John",
            LastName = "Doe",
            Address = new PayerDataAddressDto
            {
                City = "London",
            },
            Language = "EN",
            EmailAddress = "john.doe@example.com",
        },
    };
}

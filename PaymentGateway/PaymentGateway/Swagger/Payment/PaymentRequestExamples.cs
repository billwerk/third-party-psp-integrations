// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests.InvoiceData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Requests.PaymentReferenceData;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Payment;

public class PaymentRequestExamples : IMultipleExamplesProvider<PaymentRequestDto>
{
    public IEnumerable<SwaggerExample<PaymentRequestDto>> GetExamples()
    {
        yield return SwaggerExample.Create("Capture preauth request",
            "Capture preauth request (payment)",
            "Capturing payment can be triggered by billwerk if preauth transaction authorize some amount during preauth proccess." +
            "This payment request dto must contain preauth transaction id (PreauthTransactionId) which should be captured.",
            BuildSamplePaymentRequestDto()
                .To(dto =>
                {
                    dto.PreauthTransactionId = "20221108-636bd596c10cfc318b1bfabb";
                    return dto;
                }));

        yield return SwaggerExample.Create("Usual payment request", 
            "Usual payment request",
            "Can represent usual one-off / recurring payment. Doesn't contain preauth transaction id (PreauthTransactionId), " +
            "since nothing to capture.",
            BuildSamplePaymentRequestDto());

    }

    private PaymentRequestDto BuildSamplePaymentRequestDto()
        => new()
        {
            PspSettingsId = "636a915b7267328bc225c566",
            AgreementId = "636a918ca2500caaf0390a15",
            TransactionId = "20221108-636a91f61c48d9fa186166f8",
            PreauthTransactionId = "20221108-636bd596c10cfc318b1bfabb",
            RequestedAmount = 300m,
            Currency = "EUR",
            WebhookTarget = "https://sample.billwerk.com/PSPWebhooks/ExternalIntegration",
            PaymentMeansReference = new PaymentMeansReferenceDto
            {
                Role = PublicPaymentProviderRole.CreditCard,
                SuccessReturnUrl = "https://merchant.com/success",
                ErrorReturnUrl = "https://merchant.com/error",
                AbortReturnUrl = "https://merchant.com/abort",
            },
            TransactionReferenceText = "YYNPG-MZXPB-FSGYB",
            TransactionInvoiceReferenceText = "EUR 636a91f61c48d9fa186166f8",
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
            InvoiceData = new InvoiceDataDto
            {
                InvoiceDate = LocalDate.FromDateTime(DateTime.UtcNow),
                DueDate = LocalDate.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Description = "Invoice data description",
                LineItems = new List<InvoiceDataLineItemDto>()
                {
                    new()
                    {
                        TaxCategory = InvoiceDataLineTaxCategory.VatStandard,
                        Description = "Line Item description",
                        TotalGross = 300m,
                        TotalNet = 225m,
                        VatPercentage = 25,
                        PricePerUnit = 300,
                        Quantity = 1,
                        PeriodStart = DateTimeOffset.UtcNow,
                        PeriodEnd = DateTimeOffset.UtcNow.AddDays(1),
                    },
                },
            }
        };
}

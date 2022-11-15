// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests.OrderData;
using Billwerk.Payment.SDK.DTO.Requests.PayerData;
using Billwerk.Payment.SDK.DTO.Requests.PaymentReferenceData;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Preauth;

public class SendPreauthRequestExamples : IMultipleExamplesProvider<PreauthRequestDto>
{
    public IEnumerable<SwaggerExample<PreauthRequestDto>> GetExamples()
    {
        yield return SwaggerExample.Create("Credit Card",
            "Preauth request with credit card payment method",
            "Preauth request with credit card payment method. Usually, initial bearer (customer payment data) " +
            "for credit card payment is empty, because customer provide card data on PSP checkout page. " +
            "In this case, payment means reference contains only Credit Card role property plus needed after checkout page actions, urls.",
            BuildSamplePreauthRequestDto().To(preauth =>
            {
                preauth.PaymentMeansReference = BuildCreditCardPaymentMeansReference;
                return preauth;
            }));
        
        yield return SwaggerExample.Create("SEPA Direct Debit",
            "Preauth request with direct debit payment method",
            "Preauth request with direct debit payment method. Usually, initial bearer (customer payment data) " +
            "for SEPA direct debit payment contains some specific data as Holder, IBAN and etc. " +
            "In this case, payment means reference contains Debit role property plus initial customer payment data (initial bearer).",
            BuildSamplePreauthRequestDto().To(preauth =>
            {
                preauth.PaymentMeansReference = BuildDebitPaymentMeansReference;
                return preauth;
            }));

        yield return SwaggerExample.Create("Credit Card (non-initial)",
            "Non-initial preauth request with credit card payment method",
            "Non-initial preauth request with credit card payment method. Non-initial preauth request means that payment agreement " +
            "between billwerk, customer and PSP already created (represented by AgreementId). Triggered, when customer's subscription plan " +
            "was changed.",
            BuildSamplePreauthRequestDto().To(preauth =>
            {
                preauth.PaymentMeansReference = BuildCreditCardPaymentMeansReference;
                preauth.IsInitial = false;
                return preauth;
            }));
    }
    
    private PreauthRequestDto BuildSamplePreauthRequestDto()
    {
        return new PreauthRequestDto
        {
            //No matter what value, since integration flow not public yet.
            PspSettingsId = "636a915b7267328bc225c566",
            AgreementId = "636a918ca2500caaf0390a15",
            TransactionId = "20221118-636bd596c10cfc318b1bfabb",
            Currency = "EUR",
            IsInitial = true,
            RequestedAmount = 300m,
            WebhookTarget = "https://sample.billwerk.com/PSPWebhooks/ExternalIntegration",
            TransactionInvoiceReferenceText = "YYNPG-MZXPB-FSGYB",
            TransactionReferenceText = "EUR 636a91887267328bc225c5b6",
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
            OrderData = new OrderDataDto
            {
                ExternalContractId = "ExternalSystemContractId",
                FeePeriod = new OrderDataPeriodDto
                {
                    Quantity = 1,
                    Unit = TimePeriod.Month,
                },
                RecurringFee = 100m,
            },
        };
    }

    private PaymentMeansReferenceDto BuildCreditCardPaymentMeansReference => new()
    {
        Role = PublicPaymentProviderRole.CreditCard,
        SuccessReturnUrl = "https://merchant.com/success",
        ErrorReturnUrl = "https://merchant.com/error",
        AbortReturnUrl = "https://merchant.com/abort",
    };
    
    private PaymentMeansReferenceDto BuildDebitPaymentMeansReference => new()
    {
        Role = PublicPaymentProviderRole.Debit,
        InitialBearer = new Dictionary<string, string>
        {
            {"Holder", "JOHN DOE"},
            {"IBAN", "GB33BUKB20201555555555"},
        },
        SuccessReturnUrl = "https://merchant.com/success",
        ErrorReturnUrl = "https://merchant.com/error",
        AbortReturnUrl = "https://merchant.com/abort",
    };
}

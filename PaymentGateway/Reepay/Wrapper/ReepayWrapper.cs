// Copyright (c) billwerk GmbH. All rights reserved

using System.Reflection.Metadata;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using Flurl.Http;
using Flurl.Http.Configuration;
using PaymentGateway.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reepay.SDK.Models.ChargeSession;
using Reepay.SDK.Models.Charges;
using Reepay.SDK.Models.Invoices;
using Reepay.SDK.Models.PaymentMethods;
using Reepay.SDK.Models.RecurringSession;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Shared;
using Reepay.SDK.Models;
using Reepay.SDK.Models.Refund;
using Reepay.SDK.Models.Errors;
using static Reepay.Helpers.ReepayConstants;
using IFlurlClientFactory = PaymentGateway.Application.IFlurlClientFactory;

namespace Reepay.Wrapper;

public class ReepayWrapper : IDisposable
{
    private readonly IFlurlClient _apiClient, _checkoutApiClient;
    private readonly JsonSerializerSettings _serializerSettings;

    public ReepayWrapper(ReepaySettings settings, IFlurlClientFactory clientFactory)
    {
        var contractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
        _serializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver };
        var serializer = new NewtonsoftJsonSerializer(_serializerSettings);

        _apiClient = clientFactory.Create(ReepayApiUrl).Configure(ConfigureClient);
        _checkoutApiClient = clientFactory.Create(ReepayCheckoutApiUrl).Configure(ConfigureClient);

        void ConfigureClient(ClientFlurlHttpSettings httpSettings)
        {
            httpSettings.JsonSerializer = serializer;
            httpSettings.BeforeCall += call => call.Request.WithBasicAuth(settings.PrivateKey, "");
        }
    }

    public void Dispose()
    {
        _apiClient.Dispose();
        _checkoutApiClient.Dispose();
    }

    /// <summary>
    /// Create a session to charge money from customer. Produces redirect URL with payment form
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<Result<ChargeSessionResponse, PaymentErrorDto>> CreateChargeSession(CreateChargeSessionRequest request) =>
        _checkoutApiClient.Request(ChargeSessionUrl)
            .PostJsonAsync(request)
            .To(ConvertResponseTo<ChargeSessionResponse>);

    /// <summary>
    /// Perform transaction for charge created via charge session
    /// </summary>
    /// <param name="handle">Handle from <see cref="Handle"/></param>
    /// <returns>Charge object containing payment method data, customer reference, etc.</returns>
    public Task<Result<Charge, PaymentErrorDto>> GetCharge(string handle) => _apiClient.Request(ChargeBaseUrl, handle)
        .GetAsync()
        .To(ConvertResponseTo<Charge>);

    /// <summary>
    /// Perform transaction for charge created via charge session
    /// </summary>
    /// <param name="handle">Handle from <see cref="Handle"/></param>
    /// <returns>Charge object containing payment method data, customer reference, etc.</returns>
    public Task<Result<Charge, PaymentErrorDto>> SettleCharge(string handle) => _apiClient.Request(ChargeBaseUrl, handle, SettleUrl)
        .PostAsync()
        .To(ConvertResponseTo<Charge>);

    public Task<Result<InvoiceTransaction, PaymentErrorDto>> GetInvoiceTransaction(string handle, string transactionId)
        => _apiClient.Request(InvoiceBaseUrl, handle, TransactionSegmentUrl, transactionId)
            .GetAsync()
            .To(ConvertResponseTo<InvoiceTransaction>);

    /// <summary>
    /// Create charge using saved payment method (use for recurring payments)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<Result<Charge, PaymentErrorDto>> CreateCharge(CreateChargeRequest request) => _apiClient.Request(ChargeBaseUrl)
        .PostJsonAsync(request)
        .To(ConvertResponseTo<Charge>);

    /// <summary>
    /// https://docs.reepay.com/reference/createrecurringsession
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<Result<RecurringSessionResponse, PaymentErrorDto>> CreateRecurringSession(CreateRecurringSessionRequest request) =>
        _checkoutApiClient.Request(RecurringSessionUrl)
            .PostJsonAsync(request)
            .To(ConvertResponseTo<RecurringSessionResponse>);

    /// <summary>
    /// Get payment method info by it's Reepay reference. https://reference.reepay.com/api/#get-payment-method
    /// </summary>
    /// <param name="methodReference"></param>
    /// <returns></returns>
    public Task<Result<PaymentMethod, PaymentErrorDto>> GetPaymentMethod(string methodReference) =>
        _apiClient.Request(PaymentMethodsBaseUrl, methodReference)
            .GetAsync()
            .To(ConvertResponseTo<PaymentMethod>);

    /// <summary>
    /// Create refund based on existing charge
    /// </summary>
    /// <param name="refundRequest"></param>
    /// <returns></returns>
    public Task<Result<RefundResponse, PaymentErrorDto>> CreateRefund(CreateRefundRequest refundRequest) =>
        _apiClient.Request(RefundBaseUrl)
            .PostJsonAsync(refundRequest)
            .To(ConvertResponseTo<RefundResponse>);

    public async Task<Result<GetWebhookSettings, PaymentErrorDto>> GetWebhookSettingsAsync() => await _apiClient.Request(WebhookSettingsUrl)
        .GetAsync()
        .To(ConvertResponseTo<GetWebhookSettings>);

    private static async Task<Result<T, PaymentErrorDto>> ConvertResponseTo<T>(Task<IFlurlResponse> response)
    {
        try
        {
            var flurlResponse = await response;
            var responseObject = await flurlResponse.GetJsonAsync<T>();
            return responseObject;
        }
        catch (FlurlHttpTimeoutException)
        {
            return new PaymentErrorDto
            {
                ReceivedAt = DateTime.UtcNow,
                ErrorCode = PaymentErrorCode.PSPConnectionTimeout,
            };
        }
        catch (FlurlParsingException)
        {
            throw;
        }
        catch (FlurlHttpException ex)
        {
            var errorResponse = await ex.GetResponseJsonAsync<ErrorResponse>();
            return errorResponse.ToPaymentErrorDto();
        }
    }
}

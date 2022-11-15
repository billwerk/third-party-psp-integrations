// Copyright (c) billwerk GmbH. All rights reserved

using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FakeProvider;
using MicroElements.Swashbuckle.NodaTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using PaymentGateway.Application;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Infrastructure.DataAccess.Database;
using PaymentGateway.Infrastructure.DependencyInjection;
using PaymentGateway.Infrastructure.JsonSerialization;
using PaymentGateway.Infrastructure.Logging;
using PaymentGateway.Shared;
using Reepay;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway;

public static class AppBuilder
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        var container = PaymentGatewayContainer.Build(builder.Configuration);

        builder.Host.UseServiceProviderFactory(new DryIocServiceProviderFactory(container));
        builder.Services.AddHttpContextAccessor();

        var databaseSettings = (IDatabaseSettings)container.Resolve(typeof(IDatabaseSettings), DatabaseSettings.AppSettings);
        Log.Logger = DefaultLoggerConfiguration.Create(databaseSettings).CreateLogger();
        
        builder.Host.UseSerilog();

        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .AddJsonOptions(settings =>
            {
                settings.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                settings.JsonSerializerOptions.AllowTrailingCommas = true;
                settings.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                settings.JsonSerializerOptions.Converters.Clear();
                settings.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                settings.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
                settings.JsonSerializerOptions.Converters.Add(new JsonDateTimeOffsetSerializer());
                settings.JsonSerializerOptions.Converters.Add(new JsonLocalDateConverter());
            });

        builder.Services.AddMvcCore()
            .AddApiExplorer()
            .AddJsonOptions(options => ConfigureSystemTextJsonSerializerSettings(options.JsonSerializerOptions));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(swaggerGenOptions =>
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            ConfigureSystemTextJsonSerializerSettings(jsonSerializerOptions);
            swaggerGenOptions.ConfigureForNodaTimeWithSystemTextJson(jsonSerializerOptions);
            swaggerGenOptions.ExampleFilters();
            swaggerGenOptions.IncludeXmlComments(XmlCommentsFilePath);
            
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Payment Gateway",
                Description = "Following REST API documentation describes all data components that are included in the implementation of the 3rd party PSP integration. It enlists endpoints playing a major role in the process of pre-authorization, payments and refunds. " +
                              @"The additional Schema section consists of Data transfer objects (DTOs) of requests and responds with descriptions of their parameters.  
                                To have better understanding of the Payment Gateway integration, please check our public documentation [Payment Gateway](https://payment-gateaway-billwerk.gitbook.io/home/).",
                Version = "v1",
            });
        });
        builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        
        builder.Services.RegisterMediatR();
        
        CreateDefaultSettings(container, builder.Configuration);

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "Received a call to {RequestPath}";
            options.GetLevel = (_, _, ex) => ex == null ? LogEventLevel.Debug : LogEventLevel.Error;
        });

        // Configure the HTTP request pipeline.
        var isDevelopment = app.Environment.IsDevelopment();
        if (isDevelopment)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SupportedSubmitMethods();
            });
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }

    private static void CreateDefaultSettings(IContainer container, IConfiguration configuration)
    {
        const string PlaygroundEnvironmentCurrentPaymentProvider = "PlaygroundEnvironment:CurrentPaymentProvider";
        
        var currentPaymentProvider = configuration[PlaygroundEnvironmentCurrentPaymentProvider]
            .To(Convert.ToInt32)
            .To<PaymentProvider>();

        switch (currentPaymentProvider) 
        {
           case PaymentProvider.FakeProvider: 
               container.GetRequiredService<ISettingsRepository>().SaveSettings(new FakeProviderSettings());
               break;
           case PaymentProvider.Reepay:
               container.GetRequiredService<ISettingsRepository>().SaveSettings(new ReepaySettings
               {
                   PrivateKey = new NotEmptyString("private_key"),
                   WebhookSecret = new NotEmptyString("webhook_secret"),
               });
               break;
           default:
               throw new ArgumentOutOfRangeException("Unkown payment provider detected. Please, check your appsetting.json configuration" +
                                                     $"-> {PlaygroundEnvironmentCurrentPaymentProvider}");
        }

        var pspExecutionContext = new PspExecutionContext{CurrentPaymentProvider = currentPaymentProvider};
        container.RegisterInstance(pspExecutionContext);
    }
    
    private static void ConfigureSystemTextJsonSerializerSettings(JsonSerializerOptions serializerOptions)
    {
        // Configures JsonSerializer to properly serialize NodaTime types.
        serializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }
    
    static string XmlCommentsFilePath => Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.Domain.xml");
}

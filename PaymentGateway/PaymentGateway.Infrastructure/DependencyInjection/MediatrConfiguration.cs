using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Modules.Transactions.Payment.FetchPayment;
using PaymentGateway.Application.Modules.Transactions.Payment.SendPayment;
using PaymentGateway.Application.Modules.Transactions.Preauth.CancelPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.FetchPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.FinalizePreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.SendInitialPreauth;
using PaymentGateway.Application.Modules.Transactions.Preauth.SendUpgradePreauth;
using PaymentGateway.Application.Modules.Transactions.Refund.FetchRefund;
using PaymentGateway.Application.Modules.Transactions.Refund.SendRefund;

namespace PaymentGateway.Infrastructure.DependencyInjection;

public static class MediatrConfiguration
{
    public static IServiceCollection RegisterMediatR(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(typeof(SendInitialPreauthCommand));
        serviceCollection.AddMediatR(typeof(SendUpgradePreauthCommand));
        serviceCollection.AddMediatR(typeof(FetchPreauthCommand));
        serviceCollection.AddMediatR(typeof(CancelPreauthCommand));
        serviceCollection.AddMediatR(typeof(SendPaymentCommand));
        serviceCollection.AddMediatR(typeof(FetchPaymentCommand));
        serviceCollection.AddMediatR(typeof(SendRefundCommand));
        serviceCollection.AddMediatR(typeof(FetchRefundCommand));
        serviceCollection.AddMediatR(typeof(FinalizePreauthCommand));

        return serviceCollection;
    }
}

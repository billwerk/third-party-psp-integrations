using PaymentGateway.Domain.Modules;

namespace PaymentGateway.Application.Modules.PSP;

public interface IPspHandler
{
    PaymentProvider PaymentProvider { get; }
}

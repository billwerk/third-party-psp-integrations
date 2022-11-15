namespace PaymentGateway.Domain;

public interface IPaymentGatewaySettings
{
    string Domain { get; }
    string EnvironmentUrl { get; }
    string ApiUrl { get; }
}

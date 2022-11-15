namespace PaymentGateway.Domain.Shared.Abstractions.DataAccess.DatabaseObjects;

public interface IDatabaseObject<out T>
{
    T Id { get; }
}
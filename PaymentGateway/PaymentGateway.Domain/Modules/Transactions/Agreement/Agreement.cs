using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Agreement;

/// <summary>
/// This model contains all the necessary information for communication between the billwerk and
/// the payment gateway and the payment gateway and the provider.
/// For each Contract in billwerk there is a unique Agreement that will be used to create new payments.
/// </summary>
public class Agreement
{
    /// <summary>
    /// Id from <b>billwerk-platform</b> transferred with initial preauth request (<see cref="AgreementId"/>).
    /// When a payment request arrives at the payment gateway, this value is sent in the body of the request to
    /// get the appropriate agreement from the database for communication with the provider.
    /// </summary>
    public AgreementId BillwerkAgreementId { get; set; }

    /// <summary>
    /// AgreementId provided by the provider.
    /// Not all providers supports Mandates or Agreements, but all providers require some token to create new payments without customer interaction.
    /// DirectDebit and BlackLabel providers Agreement(Mandate)Id may change status and in case if status is Inactive we must notify billwerk about such change.
    /// Since we can get information that the status has changed from the notification, and it is not necessary that this notification will contain a link to the transaction in the payment gateway, we must be able to find the appropriate agreement by PspAgreementId.
    /// </summary>
    public string? PspAgreementId { get; set; }

    /// <summary>
    /// The bearer that represents customer payment data needed for requests to create new payments on providers side.
    /// </summary>
    public IDictionary<string, string> PaymentBearer { get; set; }
}

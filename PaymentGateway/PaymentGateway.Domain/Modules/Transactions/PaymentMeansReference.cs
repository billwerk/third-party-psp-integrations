namespace PaymentGateway.Domain.Modules.Transactions;

public record PaymentMeansReference
{
    /// <summary>
    /// The bearer that represents customer payment data needed for the initial call in key/value format.
    /// Optional.
    /// </summary>
    public IDictionary<string, string> InitialBearer { get; set; }

    /// <summary>
    /// The finalization URL to redirect the payer to if payment result is successful.
    /// Optional.
    /// </summary>
    public Uri SuccessReturnUrl { get; set; }

    /// <summary>
    /// The finalization URL to redirect the payer to if payment failed.
    /// Optional.
    /// </summary>
    public Uri ErrorReturnUrl { get; set; }

    /// <summary>
    /// The finalization URL to redirect the payer to if payment aborted.
    /// Optional.
    /// </summary>
    public Uri AbortReturnUrl { get; set; }
}
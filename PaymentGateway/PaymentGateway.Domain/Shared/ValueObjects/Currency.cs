using System.Globalization;
using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;

/// <summary>
/// The currency code used for the payment
/// </summary>
public record Currency : ValueObject<string>
{
    /// <summary>
    /// Constructs a new <seealso cref="Currency"/> using a ISO 4217 currency symbol.
    /// </summary>
    /// <param name="currencyCode">A ISO 4217 currency symbol.</param>
    public Currency(string currencyCode)
    {
        if (string.IsNullOrEmpty(currencyCode))
            throw new ValueObjectException<Currency>("Currency code must not be null or empty.");

        if (!IsValidCurrencyCode(currencyCode))
            throw new ValueObjectException<Currency>($"Invalid currency code '{currencyCode}'.");

        Value = currencyCode;
    }

    /// <summary>
    /// Checks if the <paramref name="currencyCode"/> is valid.
    /// This checks that it's of neutral culture, and use ISO currency symbols.
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    private static bool IsValidCurrencyCode(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return false;

        var regions = GetCultures()
            .Where(culture => !culture.IsNeutralCulture)
            .Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.Name);
                }
                catch
                {
                    return null;
                }
            });

        return regions.Any(region => region != null && region.ISOCurrencySymbol.Equals(currencyCode));
    }

    private static IEnumerable<CultureInfo> GetCultures() => CultureInfo.GetCultures(CultureTypes.AllCultures);
}

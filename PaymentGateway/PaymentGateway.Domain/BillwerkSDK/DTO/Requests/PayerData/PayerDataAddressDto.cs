namespace Billwerk.Payment.SDK.DTO.Requests.PayerData
{
    /// <summary>
    /// Represents a payer address data
    /// </summary>
    public class PayerDataAddressDto
    {
        /// <summary>
        /// The first address line.
        /// Optional.
        /// String with 64 maximum length.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The second address line.
        /// Optional.
        /// String with 64 maximum length.
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// The street.
        /// Optional.
        /// String with 64 maximum length.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// The house number.
        /// Optional.
        /// String with 12 maximum length.
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// The postal code.
        /// Mandatory.
        /// String with 12 maximum length.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// The city.
        /// Mandatory.
        /// String with 128 maximum length.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The state.
        /// Mandatory for some countries.
        /// String with 64 maximum length.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The country two-letter code.
        /// Mandatory.
        /// ISO 3166 alpha-2 format.
        /// </summary>
        public string Country { get; set; }
    }
}

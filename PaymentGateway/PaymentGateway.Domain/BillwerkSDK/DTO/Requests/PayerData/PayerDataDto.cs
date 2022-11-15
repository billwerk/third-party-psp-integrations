namespace Billwerk.Payment.SDK.DTO.Requests.PayerData
{
    /// <summary>
    /// Represents a payer data
    /// </summary>
    public class PayerDataDto
    {
        /// <summary>
        /// The value set by merchant to uniquely identify a customer per legal entity.
        /// Optional.
        /// </summary>
        public string ExternalCustomerId { get; set; }

        /// <summary>
        /// The company name.
        /// Mandatory if FirstName + LastName and EmailAddress not specified.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// The first name.
        /// Mandatory with LastName if EmailAddress and CompanyName not specified.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name.
        /// Mandatory with FirstName if EmailAddress and CompanyName not specified.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The value added tax identifier.
        /// VAT ID format is special for each country (https://www.bzst.de/)
        /// </summary>
        public string VatId { get; set; }

        /// <summary>
        /// The address.
        /// Mandatory.
        /// </summary>
        public PayerDataAddressDto Address { get; set; }

        /// <summary>
        /// The model to describe payer address (street, city, state ...).
        /// Mandatory if FirstName + LastName and CompanyName not specified.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Two-letter language code.
        /// Optional.
        /// ISO 639-1 format
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The phone number.
        /// Optional.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}

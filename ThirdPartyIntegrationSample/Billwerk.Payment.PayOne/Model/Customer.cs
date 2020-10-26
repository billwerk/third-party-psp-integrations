namespace Billwerk.Payment.PayOne.Model
{
    public class Customer : ModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }

        //optional
        public string Company { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public string Salutation { get; set; }
        public string Title { get; set; }
        public string Street { get; set; }
        public string AddressAddition { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string TelephoneNumber { get; set; }
        public string Language { get; set; }
        public string VatId { get; set; }
    }
}

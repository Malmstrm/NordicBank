using DataAccessLayer.Enums;

namespace NordicBank.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string Gender { get; set; }
        public string Givenname { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Givenname} {Surname}";

        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public DateOnly? Birthday { get; set; }
        public string? NationalId { get; set; }
        public string? Telephonecountrycode { get; set; }
        public string? Telephonenumber { get; set; }
        public string? Phone => $"+{Telephonecountrycode} {Telephonenumber}";
        public string? Emailaddress { get; set; }

        public CustomerStatus Status { get; set; }

        // Extra fält för kundbild:
        public int NumberOfAccounts { get; set; }
        public decimal TotalBalance { get; set; }
    }
}

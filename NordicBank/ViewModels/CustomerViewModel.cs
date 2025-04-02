using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace NordicBank.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string Givenname { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        [Display(Name = "Street Address")]
        public string Streetaddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [Display(Name = "Zip Code")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Country code is required.")]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        public DateOnly? Birthday { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [Display(Name = "National ID")]
        public string? NationalId { get; set; }

        [Required(ErrorMessage = "Telephone country code is required.")]
        [Display(Name = "Telephone Country Code")]
        public string? Telephonecountrycode { get; set; }

        [Required(ErrorMessage = "Telephone number is required.")]
        [Display(Name = "Telephone Number")]
        public string? Telephonenumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email Address")]
        public string? Emailaddress { get; set; }
        public CustomerStatus Status { get; set; }
    }
}

using DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace NordicBank.ViewModels
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        public string Frequency { get; set; } = string.Empty;

        public DateOnly Created { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive number.")]
        public decimal Balance { get; set; }

        public AccountStatus AccountStatus { get; set; }

        [Required]
        public int CustomerId { get; set; }

    }
}

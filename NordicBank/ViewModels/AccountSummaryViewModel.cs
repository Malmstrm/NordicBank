using DataAccessLayer.Enums;

namespace NordicBank.ViewModels
{
    public class AccountSummaryViewModel
    {
        public int AccountId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public DateOnly Created { get; set; }

        public decimal Balance { get; set; }

        public string Frequency { get; set; } = string.Empty;

        public AccountStatus AccountStatus { get; set; }
    }
}

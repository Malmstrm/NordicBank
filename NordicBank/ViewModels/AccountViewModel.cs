using DataAccessLayer.Enums;

namespace NordicBank.ViewModels
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; }
        public DateOnly Created {  get; set; }
        public decimal Balance { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public int CustomerId { get; set; }

    }
}

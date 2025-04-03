using DataAccessLayer.Enums;

namespace DataAccessLayer.DTO
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; }
        public DateOnly Created { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}

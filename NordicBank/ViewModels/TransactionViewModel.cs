namespace NordicBank.ViewModels
{
    public class TransactionViewModel
    {
        public DateOnly Date {  get; set; }
        public string Type { get; set; } = "";
        public string Operation { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public int TransactionId { get; set; }
        public int AccountId { get; set; }

    }
}

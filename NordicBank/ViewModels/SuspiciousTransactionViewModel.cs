namespace NordicBank.ViewModels
{
    public class SuspiciousTransactionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

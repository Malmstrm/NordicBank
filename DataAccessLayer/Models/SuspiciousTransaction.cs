using DataAccessLayer.Enums;

namespace DataAccessLayer.Models
{

    public class SuspiciousTransaction
    {
        public int Id { get; set; }

        public int ScanLogId { get; set; }
        public ScanLog ScanLog { get; set; } = null!;

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public SuspicionReason Reason { get; set; }
    }
}

namespace DataAccessLayer.Models
{
    public class ScanLog
    {
        public int Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspiciousCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SuspiciousTransaction> SuspiciousTransactions { get; set; } = new List<SuspiciousTransaction>();
    }
}

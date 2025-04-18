namespace DataAccessLayer.DTO
{
    public class ScanResultDTO
    {
        public string Country { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspiciousCount { get; set; }
        public List<SuspiciousTransactionDTO> Transactions { get; set; } = new();
    }
}

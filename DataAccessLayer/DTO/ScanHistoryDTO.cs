namespace DataAccessLayer.DTO
{
    public class ScanHistoryDTO
    {
        public string Country { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspiciousCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

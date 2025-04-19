namespace NordicBank.ViewModels
{
    public class ScanLogViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspiciousCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

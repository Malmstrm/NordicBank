using DataAccessLayer.Models;

namespace Services
{
    public interface IScanLogRepository
    {
        Task<DateTime> LoadLastScanDateAsync(string country);
        Task<ScanLog> SaveScanLogAsync(string country, DateTime start, DateTime end, List<SuspiciousTransaction> transactions);
    }
}

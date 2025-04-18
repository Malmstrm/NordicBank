using DataAccessLayer.DTO;
using DataAccessLayer.Models;

namespace Services
{
    public interface IScanLogRepository
    {
        Task<DateTime> GetLastScanDateAsync(string country);
        Task SaveScanLogAsync(string country, DateTime from, DateTime to, List<SuspiciousTransaction> transactions);
        Task<List<ScanHistoryDTO>> GetScanHistoryAsync(string country);
    }
}

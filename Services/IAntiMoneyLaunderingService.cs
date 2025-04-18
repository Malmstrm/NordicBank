using DataAccessLayer.DTO;

namespace Services
{
    public interface IAntiMoneyLaunderingService
    {
        Task<ScanResultDTO> RunScanAsync(string country, DateTime startDate, DateTime endDate);
        Task<DateTime> GetEarliestTransactionDateAsync(string country);
        Task<DateTime> GetLastScanDateAsync(string country); // NY
    }
}

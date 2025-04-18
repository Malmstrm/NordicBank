using DataAccessLayer.DTO;

namespace Services
{
    public interface IAntiMoneyLaunderingService
    {
        Task<ScanLogDTO> RunScanAsync(string country, DateTime endDate);
        Task<DateTime> GetEarliestTransactionDateAsync(string country);
    }
}

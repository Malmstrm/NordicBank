using DataAccessLayer.DTO;

namespace Services
{
    public interface IAntiMoneyLaunderingService
    {
        Task<ScanResultDTO> RunScanAsync(string country, DateTime endDate);
        Task<DateTime> GetEarliestTransactionDateAsync(string country);
    }
}

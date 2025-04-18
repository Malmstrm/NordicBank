using DataAccessLayer.DTO;

namespace Services
{
    public interface ICountryOverviewService
    {
        Task<List<CountryOverviewDTO>> GetCountryOverviewAsync();
        Task<List<TopCustomerDTO>> GetTopCustomersByCountryAsync(string country);
        Task<List<CountryReportDTO>> GetDetailedCountryReportAsync();
        Task<CustomerActivityDTO> GetCustomerActivityAsync();
    }
}

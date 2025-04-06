using DataAccessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICountryOverviewService
    {
        Task<List<CountryOverviewDTO>> GetCountryOverviewAsync();
        Task<List<TopCustomerDTO>> GetTopCustomersByCountryAsync(string country);
    }
}

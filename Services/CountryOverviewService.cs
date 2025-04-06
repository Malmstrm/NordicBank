using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Services
{
    public class CountryOverviewService : ICountryOverviewService
    {
        private readonly NordicBankAppDataContext _dbContext;

        public CountryOverviewService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CountryOverviewDTO>> GetCountryOverviewAsync()
        {
            var selectedCountries = new[] { "Sweden", "Norway", "Finland", "Denmark" };

            return await _dbContext.Customers
                .Where(c => selectedCountries.Contains(c.Country))
                .GroupBy(c => c.Country)
                .Select(g => new CountryOverviewDTO()
                {
                    Country = g.Key,
                    CountryCode = g.Select(c => c.CountryCode).FirstOrDefault(),
                    Clients = g.Count(),
                    Accounts = g.SelectMany(c => c.Dispositions)
                        .Where(d => d.Type == "OWNER")
                        .Select(d => d.AccountId)
                        .Distinct()
                        .Count(),
                    Capital = g.SelectMany(c => c.Dispositions)
                        .Select(d => d.Account)
                        .GroupBy(a => a.AccountId)
                        .Select(ag => ag.FirstOrDefault().Balance)
                        .Sum()
                })
                .ToListAsync();

        }
    }
}

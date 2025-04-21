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
        public async Task<List<CountryReportDTO>> GetDetailedCountryReportAsync()
        {
            var selectedCountries = new[] { "Sweden", "Norway", "Finland", "Denmark" };

            var baseData = await _dbContext.Customers
                .Where(c => selectedCountries.Contains(c.Country))
                .GroupBy(c => c.Country)
                .Select(g => new CountryReportDTO
                {
                    Country = g.Key,
                    CountryCode = g.Select(c => c.CountryCode).FirstOrDefault(),

                    // ✅ Endast kunder med minst en OWNER-disposition
                    Clients = g.Count(c => c.Dispositions.Any(d => d.Type == "OWNER")),

                    // ✅ Alla konton (även flera per kund) där kunden är OWNER
                    Accounts = g.SelectMany(c => c.Dispositions)
                                .Where(d => d.Type == "OWNER")
                                .Select(d => d.AccountId)
                                .Count(),

                    // ✅ Total balans på dessa konton
                    Capital = g.SelectMany(c => c.Dispositions)
                                .Where(d => d.Type == "OWNER")
                                .Select(d => d.Account.Balance)
                                .Sum()
                })
                .ToListAsync();

            // Lägg till toppkund per land
            foreach (var item in baseData)
            {
                var topCustomer = await _dbContext.Customers
                    .Where(c => c.Country == item.Country)
                    .Select(c => new
                    {
                        Name = c.Givenname + " " + c.Surname,
                        Balance = c.Dispositions
                                  .Where(d => d.Type == "OWNER")
                                  .Select(d => d.Account.Balance)
                                  .Sum()
                    })
                    .OrderByDescending(c => c.Balance)
                    .FirstOrDefaultAsync();

                item.TopCustomerName = topCustomer?.Name;
                item.TopCustomerBalance = topCustomer?.Balance;
            }

            return baseData;
        }
        public async Task<CustomerActivityDTO> GetCustomerActivityAsync()
        {
            var now = DateTime.Now;

            var activeSince = DateOnly.FromDateTime(now.AddDays(-90));
            var inactiveSince = DateOnly.FromDateTime(now.AddYears(-1));
            var newSince = DateOnly.FromDateTime(now.AddDays(-30));

            // 🟢 Aktiva kunder = Har gjort minst en transaktion senaste 90 dagarna
            var activeCustomerIds = await _dbContext.Dispositions
                .Where(d => d.Type == "OWNER")
                .Join(_dbContext.Transactions,
                    d => d.AccountId,
                    t => t.AccountId,
                    (d, t) => new { d.CustomerId, t.Date })
                .Where(x => x.Date >= activeSince)
                .Select(x => x.CustomerId)
                .Distinct()
                .ToListAsync();

            // 🔴 Kunder som gjort minst en transaktion senaste året
            var recentlyActiveCustomerIds = await _dbContext.Dispositions
                .Where(d => d.Type == "OWNER")
                .Join(_dbContext.Transactions,
                    d => d.AccountId,
                    t => t.AccountId,
                    (d, t) => new { d.CustomerId, t.Date })
                .Where(x => x.Date >= inactiveSince)
                .Select(x => x.CustomerId)
                .Distinct()
                .ToListAsync();

            // Alla kunder som har minst en OWNER-disposition
            var ownerCustomerIds = await _dbContext.Dispositions
                .Where(d => d.Type == "OWNER")
                .Select(d => d.CustomerId)
                .Distinct()
                .ToListAsync();

            // Inaktiva = Har minst ett konto men INGEN transaktion senaste året
            var inactiveCustomerIds = ownerCustomerIds
                .Except(recentlyActiveCustomerIds)
                .ToList();

            // 🆕 Nya kunder = Registrerade senaste 30 dagar (använder CreatedAt)
            var newCustomerCount = await _dbContext.Customers
                .Where(c => c.CreatedAt >= now.AddDays(-30))
                .CountAsync();

            return new CustomerActivityDTO
            {
                ActiveCustomers = activeCustomerIds.Count,
                InactiveCustomers = inactiveCustomerIds.Count,
                NewCustomers = newCustomerCount
            };
        }

        public async Task<List<TopCustomerDTO>> GetTopCustomersByCountryAsync(string country)
        {
            return await _dbContext.Customers
                .Where(c => c.Country == country)
                .Select(c => new TopCustomerDTO
                {
                    CustomerId = c.CustomerId,
                    Name = c.Givenname + " " + c.Surname,
                    City = c.City,
                    TotalBalance = c.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => d.Account.Balance)
                        .Sum()
                })
                .OrderByDescending(c => c.TotalBalance)
                .Take(10)
                .ToListAsync();
        }
    }
}

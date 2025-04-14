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

            // Konvertera till DateOnly för jämförelser med kolumner som är DateOnly
            var activeSince = DateOnly.FromDateTime(now.AddDays(-90));
            var inactiveSince = DateOnly.FromDateTime(now.AddYears(-1));
            var newSince = DateOnly.FromDateTime(now.AddDays(-30));

            // Aktiva kunder = Har gjort minst en transaktion senaste 90 dagarna
            var activeCustomerIds = await _dbContext.Transactions
                .Where(t => t.Date >= activeSince)
                .Select(t => t.AccountId)
                .Distinct()
                .Join(
                    _dbContext.Dispositions.Where(d => d.Type == "OWNER"),
                    accountId => accountId,
                    d => d.AccountId,
                    (accountId, d) => d.CustomerId
                )
                .Distinct()
                .ToListAsync();

            // Inaktiva kunder = Ej gjort någon transaktion senaste året
            var recentlyActiveCustomerIds = await _dbContext.Dispositions
                .Where(d => d.Type == "OWNER")
                .Where(d => _dbContext.Transactions
                    .Any(t => t.AccountId == d.AccountId && t.Date >= inactiveSince))
                .Select(d => d.CustomerId)
                .Distinct()
                .ToListAsync();

            var allCustomerIds = await _dbContext.Customers
                .Select(c => c.CustomerId)
                .ToListAsync();

            var inactiveCustomerIds = allCustomerIds
                .Except(recentlyActiveCustomerIds)
                .ToList();

            // Nya kunder = Registrerade (Birthday) senaste 30 dagar
            var newCustomerCount = await _dbContext.Customers
                .Where(c => c.Birthday >= newSince)
                .CountAsync();

            return new CustomerActivityDTO
            {
                ActiveCustomers = activeCustomerIds.Count(),
                InactiveCustomers = inactiveCustomerIds.Count(),
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

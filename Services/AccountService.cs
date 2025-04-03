using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly NordicBankAppDataContext _dbContext;

        public AccountService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AccountDTO>> GetCustomerAccountAsync(int customerId)
        {
            return await _dbContext.Dispositions
                .Where(a => a.CustomerId == customerId)
                .Select(a => new AccountDTO()
                {
                    AccountId = a.Account.AccountId,
                    Frequency = a.Account.Frequency,
                    Created = a.Account.Created,
                    Balance = a.Account.Balance,
                    AccountStatus = a.Account.AccountStatus,
                    CustomerId = a.CustomerId
                })
                .ToListAsync();

        }
        public async Task<AccountDTO?> GetAccountByIDAsync(int accountId)
        {
            return await _dbContext.Dispositions
                .Where(d => d.AccountId == accountId)
                .Select(d => new AccountDTO
                {
                    AccountId = d.Account.AccountId,
                    Frequency = d.Account.Frequency,
                    Created = d.Account.Created,
                    Balance = d.Account.Balance,
                    AccountStatus = d.Account.AccountStatus,
                    CustomerId = d.CustomerId
                })
                .FirstOrDefaultAsync();
        }
    }
}

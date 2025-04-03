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
                })
                .ToListAsync();

        }
    }
}

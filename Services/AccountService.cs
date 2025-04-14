using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
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
        public async Task<bool> UpdateStatusAsync(int accountId, AccountStatus newStatus)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if(account == null) return false;

            account.AccountStatus = newStatus;
            await _dbContext.SaveChangesAsync();
            return true;
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
        public async Task<int> GetAccountCountAsync(int customerId)
        {
            return await _dbContext.Dispositions
                .Where(d => d.CustomerId == customerId && d.Type == "OWNER")
                .Select(d => d.AccountId)
                .Distinct()
                .CountAsync();
        }

        public async Task<decimal> GetTotalBalanceAsync(int customerId)
        {
            return await _dbContext.Dispositions
                .Where(d => d.CustomerId == customerId && d.Type == "OWNER")
                .Select(d => d.Account.Balance)
                .SumAsync();
        }
        public async Task<List<AccountSummaryDTO>> GetAllAccountSummariesAsync()
        {
            return await _dbContext.Dispositions
                .Where(d => d.Type == "OWNER")
                .Select(d => new AccountSummaryDTO
                {
                    AccountId = d.AccountId,
                    CustomerName = d.Customer.Givenname + " " + d.Customer.Surname,
                    Created = d.Account.Created,
                    Balance = d.Account.Balance,
                    Frequency = d.Account.Frequency,
                    AccountStatus = d.Account.AccountStatus
                })
                .ToListAsync();
        }

    }
}

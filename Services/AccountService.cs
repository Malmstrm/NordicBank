using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly NordicBankAppDataContext _dbContext;
        private readonly IMapper _mapper;

        public AccountService(NordicBankAppDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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
            var accounts = await _dbContext.Dispositions
                .Where(d => d.CustomerId == customerId)
                .Select(d => new
                {
                    Account = d.Account,
                    CustomerId = d.CustomerId
                })
                .AsNoTracking()
                .ToListAsync();

            var accountDtos = accounts.Select(a =>
            {
                var dto = _mapper.Map<AccountDTO>(a.Account);
                dto.CustomerId = a.CustomerId;
                return dto;
            }).ToList();

            return accountDtos;
        }
        public async Task<AccountDTO?> GetAccountByIDAsync(int accountId)
        {
            var result = await _dbContext.Dispositions
                .Where(d => d.AccountId == accountId)
                .Select(d => new
                {
                    Account = d.Account,
                    CustomerId = d.CustomerId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null) return null;

            var dto = _mapper.Map<AccountDTO>(result.Account);
            dto.CustomerId = result.CustomerId;
            return dto;
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
                    AccountStatus = d.Account.AccountStatus,
                })
                .ToListAsync();
        }
        public async Task<List<AccountSummaryDTO>> GetAccountSummaryListAsync()
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
                    AccountStatus = d.Account.AccountStatus,
                    CustomerId = d.CustomerId,
                })
                .ToListAsync();
        }
        public async Task<int> CreateAccount(int customerId, string frequency)
        {
            var account = new Account()
            {
                Created = DateOnly.FromDateTime(DateTime.Now),
                Frequency = frequency,
                Balance = 0,
                AccountStatus = AccountStatus.Inactive,
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            var disposition = new Disposition()
            {
                CustomerId = customerId,
                AccountId = account.AccountId,
                Type = "OWNER"
            };

            _dbContext.Dispositions.Add(disposition);
            await _dbContext.SaveChangesAsync();

            return account.AccountId;
        }
    }
}

using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class TransactionService : ITransactionService
    {
        private readonly NordicBankAppDataContext _dbContext;

        public TransactionService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TransactionDTO>> GetTransactionsIdAsync(int accountId)
        {
            return await _dbContext.Transactions
                .Where(t => t.AccountId == accountId)
                .Select(t => new TransactionDTO()
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Description = t.Symbol ?? t.Bank ?? t.Account ?? t.Operation,
                    AccountId = accountId
                })
                .ToListAsync();
        }
        public async Task<List<TransactionDTO>> GetLatestTransactionsCustomer(int customerId)
        {
            return await _dbContext.Transactions
                .Where(t => _dbContext.Dispositions
                    .Any(d => d.CustomerId == customerId && d.AccountId == d.AccountId))
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Take(10)
                .Select(t => new TransactionDTO()
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Description = t.Symbol ?? t.Operation,
                    AccountId = t.AccountId
                })
                .ToListAsync();
        }
    }
}

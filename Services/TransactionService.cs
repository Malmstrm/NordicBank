using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Services.Utility;


namespace Services
{
    public class TransactionService : ITransactionService
    {
        private readonly NordicBankAppDataContext _dbContext;

        public TransactionService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<TransactionResult> DepositAsync(int accountId, decimal amount)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if (account == null) 
                return TransactionResult.Failed("Account not found");
            if (account.AccountStatus != DataAccessLayer.Enums.AccountStatus.Active)
                return TransactionResult.Failed("Account not active.");

            await UpdateBalanceAndLogTransactionAsync(accountId, amount, "Credit", "Deposit", "Bank");
            return TransactionResult.Ok();

        }
        public async Task<TransactionResult> WithdrawAsync(int accountId, decimal amount)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if (account == null)
                return TransactionResult.Failed("Account not found");
            if (account.AccountStatus != DataAccessLayer.Enums.AccountStatus.Active)
                return TransactionResult.Failed("Account not active.");

            await UpdateBalanceAndLogTransactionAsync(accountId, amount, "Debit", "Withdraw", "Bank");
            return TransactionResult.Ok();
        }
        public async Task<bool> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromAccount = await _dbContext.Accounts.FindAsync(fromAccountId);
            var toAccount = await _dbContext.Accounts.FindAsync(toAccountId);

            if (fromAccount == null || toAccount == null) return false;

            if(fromAccount.AccountStatus != DataAccessLayer.Enums.AccountStatus.Active || toAccount.AccountStatus != DataAccessLayer.Enums.AccountStatus.Active) return false;

            if(fromAccount.Balance < amount) return false;

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            await CreateTransactionAsync(fromAccountId, "Debit", "Transfer to " + toAccountId, -amount, fromAccount.Balance, "Transfer");
            await CreateTransactionAsync(toAccountId, "Credit", "Transfer from " + fromAccountId, amount, toAccount.Balance, "Transfer");

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<TransactionDTO>> GetTransactionsIdAsync(int accountId)
        {
            return await _dbContext.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
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
                    .Any(d => d.CustomerId == customerId && d.AccountId == t.AccountId))
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Take(10)
                .Select(t => new TransactionDTO
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
        public async Task CreateTransactionAsync(int accountId, string type, string operation, decimal amount, decimal balance, string? symbol = null)
        {
            var transaction = new Transaction
            {
                AccountId = accountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = type,
                Operation = operation,
                Amount = amount,
                Balance = balance,
                Symbol = symbol
            };

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }
        private async Task<bool> UpdateBalanceAndLogTransactionAsync(
            int accountId, decimal amount, string type, string operation, string symbol)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if(account == null || account.AccountStatus != DataAccessLayer.Enums.AccountStatus.Active) return false;

            if(type == "Debit" && account.Balance < amount) return false;

            account.Balance += (type == "Credit" ? amount : -amount);

            await CreateTransactionAsync(
                accountId,
                type,
                operation,
                type == "Credit" ? amount : -amount,
                account.Balance,
                symbol);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

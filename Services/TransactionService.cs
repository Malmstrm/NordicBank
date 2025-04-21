using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Services.Utility;

namespace Services
{
    public class TransactionService : ITransactionService
    {
        private readonly NordicBankAppDataContext _dbContext;
        private readonly IMapper _mapper;

        public TransactionService(NordicBankAppDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<TransactionResult> DepositAsync(int accountId, decimal amount)
        {
            var validation = await ValidateTransactionAsync(accountId);
            if (!validation.IsValid) return TransactionResult.Failed(validation.ErrorMessage);

            await UpdateBalanceAndLogTransactionAsync(accountId, amount, "Credit", "Deposit", "Bank");
            return TransactionResult.Ok();
        }

        public async Task<TransactionResult> WithdrawAsync(int accountId, decimal amount)
        {
            var validation = await ValidateTransactionAsync(accountId);
            if (!validation.IsValid) return TransactionResult.Failed(validation.ErrorMessage);

            await UpdateBalanceAndLogTransactionAsync(accountId, amount, "Debit", "Withdraw", "Bank");
            return TransactionResult.Ok();
        }

        public async Task<TransactionResult> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromValidation = await ValidateTransactionAsync(fromAccountId);
            if (!fromValidation.IsValid) return TransactionResult.Failed("Sender: " + fromValidation.ErrorMessage);

            var toValidation = await ValidateTransactionAsync(toAccountId);
            if (!toValidation.IsValid) return TransactionResult.Failed("Receiver: " + toValidation.ErrorMessage);

            var fromAccount = await _dbContext.Accounts.FindAsync(fromAccountId);
            var toAccount = await _dbContext.Accounts.FindAsync(toAccountId);

            if (fromAccount == null || toAccount == null)
                return TransactionResult.Failed("One or both accounts were not found.");

            if (fromAccount.Balance < amount)
                return TransactionResult.Failed("Insufficient funds.");

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            await CreateTransactionAsync(fromAccountId, "Debit", $"Transfer to {toAccountId}", -amount, fromAccount.Balance, "Transfer");
            await CreateTransactionAsync(toAccountId, "Credit", $"Transfer from {fromAccountId}", amount, toAccount.Balance, "Transfer");

            await _dbContext.SaveChangesAsync();
            return TransactionResult.Ok();
        }

        public async Task<List<TransactionDTO>> GetTransactionsIdAsync(int accountId)
        {
            var transactions = await _dbContext.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TransactionDTO>>(transactions);
        }

        public async Task<List<TransactionDTO>> GetTransactionsPagedAsync(int accountId, int skip, int take = 20)
        {
            var transactions = await _dbContext.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TransactionDTO>>(transactions);
        }

        public async Task<List<TransactionDTO>> GetLatestTransactionsCustomer(int customerId)
        {
            var transactions = await _dbContext.Transactions
                .Where(t => _dbContext.Dispositions
                    .Any(d => d.CustomerId == customerId && d.AccountId == t.AccountId))
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<TransactionDTO>>(transactions);
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
            if (account == null || account.AccountStatus != AccountStatus.Active) return false;

            if (type == "Debit" && account.Balance < amount) return false;

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

        public async Task<ValidationResult> ValidateTransactionAsync(int accountId)
        {
            var disposition = await _dbContext.Dispositions
                .Include(d => d.Customer)
                .Include(d => d.Account)
                .FirstOrDefaultAsync(d => d.AccountId == accountId && d.Type == "OWNER");

            if (disposition == null) return ValidationResult.Invalid("Account or customer not found.");

            if (disposition.Account.AccountStatus != AccountStatus.Active) return ValidationResult.Invalid("Account is not active.");

            if (disposition.Customer.CustomerStatus != CustomerStatus.Active) return ValidationResult.Invalid("Customer is not active");

            return ValidationResult.Valid();
        }
    }
}

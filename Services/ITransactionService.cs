using DataAccessLayer.DTO;
using Services.Utility;

namespace Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDTO>> GetTransactionsIdAsync(int accountId);
        Task<List<TransactionDTO>> GetLatestTransactionsCustomer(int customerId);
        Task<TransactionResult> DepositAsync(int accountId, decimal amount);
        Task<TransactionResult> WithdrawAsync(int accountId, decimal amount);
        Task CreateTransactionAsync(int accountId, string type, string operation, decimal amount, decimal balance, string? symbol = null);
        Task<TransactionResult> TransferAsync(int fromAccount, int toAccount, decimal amount);
        Task<List<TransactionDTO>> GetTransactionsPagedAsync(int accountId, int skip, int take = 20);
    }
}

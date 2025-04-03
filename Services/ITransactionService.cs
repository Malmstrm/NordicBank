using DataAccessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDTO>> GetTransactionsIdAsync(int accountId);
        Task<List<TransactionDTO>> GetLatestTransactionsCustomer(int customerId);
        Task<bool> DepositAsync(int accountId, decimal amount);
        Task<bool> WithdrawAsync(int accountId, decimal amount);
        Task CreateTransactionAsync(int accountId, string type, string operation, decimal amount, decimal balance, string? symbol = null);
        Task<bool> TransferAsync(int fromAccount, int toAccount, decimal amount);
    }
}

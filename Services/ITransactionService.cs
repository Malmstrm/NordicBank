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
    }
}

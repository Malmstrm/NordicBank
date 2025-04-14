using DataAccessLayer.DTO;
using DataAccessLayer.Enums;

namespace Services
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetCustomerAccountAsync(int customerId);
        Task<AccountDTO?> GetAccountByIDAsync(int accountId);
        Task<bool> UpdateStatusAsync(int accountId, AccountStatus newStatus);
        Task<int> GetAccountCountAsync(int customerId);
        Task<decimal> GetTotalBalanceAsync(int customerId);
        Task<List<AccountSummaryDTO>> GetAllAccountSummariesAsync();
        Task<List<AccountSummaryDTO>> GetAccountSummaryListAsync();


    }
}

using DataAccessLayer.DTO;

namespace Services
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetCustomerAccountAsync(int customerId);
        Task<AccountDTO?> GetAccountByIDAsync(int accountId);
    }
}

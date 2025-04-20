using DataAccessLayer.DTO;
using DataAccessLayer.Enums;

namespace Services
{
    public interface ICustomerService
    {
        Task<List<ViewCustomerDTO>> GetViewAsync();
        Task<CustomerDTO> GetByIdAsync(int customerId);
        Task<CustomerDTO> CreateAsync(CustomerDTO dto);
        Task<CustomerDTO> UpdateAsync(CustomerDTO dto);
        Task<bool> DeleteAsync(int customerId);
        Task<bool> UpdateStatusAsync(int customerId, CustomerStatus newStatus);
    }
}

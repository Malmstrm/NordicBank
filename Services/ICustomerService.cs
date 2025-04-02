
using DataAccessLayer.DTO;

namespace Services
{
    public interface ICustomerService
    {
        Task<List<ViewCustomerDTO>> GetViewAsync();
        Task<CustomerDTO> GetByIdAsyn(int customerId);
        Task<CustomerDTO> CreateAsync(CustomerDTO dto);
        Task<CustomerDTO> UpdateAsync(CustomerDTO dto);
    }
}

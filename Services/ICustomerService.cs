
using DataAccessLayer.DTO;

namespace Services
{
    public interface ICustomerService
    {
        Task<List<ViewCustomerDTO>> GetViewAsync();
    }
}

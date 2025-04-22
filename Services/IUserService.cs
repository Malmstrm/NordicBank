using DataAccessLayer.DTO;

namespace Services
{
    public interface IUserService
    {
        Task<UserListDTO> GetUsersPagedAsync(string? sortOrder, int page, int pageSize);
    }
}

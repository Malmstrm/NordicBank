using DataAccessLayer.DTO;

namespace Services
{
    public interface IUserService
    {
        Task<UserListDTO> GetUsersPagedAsync(string? sortOrder, int page, int pageSize);
        Task<UserEditDTO?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(UserEditDTO dto);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ToggleUserStatusAsync(string userId);
    }
}

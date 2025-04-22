using AutoMapper;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserListDTO> GetUsersPagedAsync(string? sortOrder, int page, int pageSize)
        {
            var query = _userManager.Users.AsQueryable();

            query = sortOrder switch
            {
                "username_desc" => query.OrderByDescending(u => u.UserName),
                "email" => query.OrderBy(u => u.Email),
                "email_desc" => query.OrderByDescending(u => u.Email),
                _ => query.OrderBy(u => u.UserName),
            };

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = new List<UserDTO>();

            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDTO>(user);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault() ?? "-"; // Anta att de har 1 roll
                dtoList.Add(dto);
            }

            return new UserListDTO
            {
                Users = dtoList,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                CurrentSort = sortOrder
            };
        }
        public async Task<UserEditDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            return new UserEditDTO
            {
                Id = user.Id,
                Email = user.Email ?? "",
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now,
                Role = role
            };
        }
        public async Task<bool> UpdateUserAsync(UserEditDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return false;

            user.Email = dto.Email;
            user.UserName = dto.Email;

            // Lockout / unlock
            user.LockoutEnd = dto.IsActive ? null : DateTimeOffset.MaxValue;

            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> DeleteUserAsync(string userId, string currentUserId)
        {
            // 👮 Förhindra att en admin raderar sig själv
            if (userId == currentUserId)
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Toggle LockoutEnd (det du använder som "inaktiv")
            if (user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now)
                user.LockoutEnd = DateTimeOffset.MaxValue; // Inaktivera
            else
                user.LockoutEnd = null; // Aktivera

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

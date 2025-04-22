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

    }
}

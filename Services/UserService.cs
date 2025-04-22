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
                "email_desc" => query.OrderByDescending(u => u.Email),
                _ => query.OrderBy(u => u.Email),
            };

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = _mapper.Map<List<UserDTO>>(users);

            return new UserListDTO()
            {
                Users = dtoList,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SortOrder = sortOrder
            };
        }

    }
}

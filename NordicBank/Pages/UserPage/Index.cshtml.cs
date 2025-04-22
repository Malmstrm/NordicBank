using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.UserPage
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public IndexModel(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public UserListViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            const int pageSize = 10;

            var dto = await _userService.GetUsersPagedAsync(SortOrder, Page, pageSize);
            ViewModel = _mapper.Map<UserListViewModel>(dto);

            return Page();
        }
    }
}

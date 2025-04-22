using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using NordicBank.ViewModels;
using Services;
using System.Security.Claims;

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
        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchEmail { get; set; }
        public UserListViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            const int pageSize = 10;

            var dto = await _userService.GetUsersPagedAsync(SortOrder, Page, pageSize, SearchEmail);
            ViewModel = _mapper.Map<UserListViewModel>(dto);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _userService.DeleteUserAsync(id, currentUserId);

            StatusMessage = success
                ? "✅ User deleted successfully."
                : "❌ Could not delete user (maybe you're trying to delete yourself?).";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(string id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync();

            var success = await _userService.ToggleUserStatusAsync(id);
            if (!success)
                ModelState.AddModelError(string.Empty, "Could not update user status.");

            return RedirectToPage();
        }
    }
}

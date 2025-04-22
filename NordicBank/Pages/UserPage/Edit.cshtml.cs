using AutoMapper;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.UserPage
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EditModel(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [BindProperty]
        public UserEditViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var dto = await _userService.GetUserByIdAsync(id);
            if (dto == null) return NotFound();

            ViewModel = _mapper.Map<UserEditViewModel>(dto);
            ViewModel.AvailableRoles = new List<string> { "Admin", "Cashier" };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewModel.AvailableRoles = new List<string> { "Admin", "Cashier" };
                return Page();
            }

            var dto = _mapper.Map<UserEditDTO>(ViewModel);
            var success = await _userService.UpdateUserAsync(dto);

            if (!success) return NotFound();

            return RedirectToPage("./Index");
        }
    }
}

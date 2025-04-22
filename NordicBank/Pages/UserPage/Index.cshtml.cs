using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NordicBank.Pages.UserPage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly IUserService _userService; // valfritt service-lager om du använder det

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "None",
                    IsActive = true // du kan anpassa detta baserat på en IsActive-property
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToPage();
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
    }

}

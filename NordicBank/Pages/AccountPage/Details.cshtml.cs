using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.AccountPage
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DetailsModel(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        [BindProperty(SupportsGet = true)] public int CustomerId { get; set; }

        public AccountViewModel Account {  get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var dto = await _accountService.GetAccountByIDAsync(Id);
            if(dto == null) return NotFound();

            Account = new AccountViewModel
            {
                AccountId = dto.AccountId,
                Frequency = dto.Frequency,
                Created = dto.Created,
                Balance = dto.Balance,
                AccountStatus = dto.AccountStatus,
                CustomerId = dto.CustomerId,

            };


            return Page();
        }
    }
}

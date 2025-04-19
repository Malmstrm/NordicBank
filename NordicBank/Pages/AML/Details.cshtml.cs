using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.AML
{
    public class DetailsModel : PageModel
    {
        private readonly IAntiMoneyLaunderingService _amlService;

        public DetailsModel(IAntiMoneyLaunderingService amlService)
        {
            _amlService = amlService;
        }

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public DateTime From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime To { get; set; }

        public List<SuspiciousTransactionDTO> SuspiciousTransactions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Country) || From == default || To == default)
            {
                return NotFound("Missing or invalid query parameters.");
            }

            SuspiciousTransactions = await _amlService.GetSuspiciousTransactionsAsync(Country, From, To);
            return Page();
        }
    }
}

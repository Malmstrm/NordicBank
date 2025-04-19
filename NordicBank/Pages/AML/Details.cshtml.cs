using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NordicBank.Infrastructure.Paging.Country;
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

        public List<SuspiciousTransactionDTO> Transactions { get; set; } = new();

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int CurrentPage => PageNo;

        public List<SuspiciousTransactionDTO> PagedTransactions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int pageNo = 1)
        {
            if (string.IsNullOrEmpty(Country) || From == default || To == default)
                return NotFound();

            PageNo = pageNo;

            Transactions = await _amlService.GetSuspiciousTransactionsAsync(Country, From, To);
            TotalPages = (int)Math.Ceiling(Transactions.Count / (double)PageSize);

            PagedTransactions = Transactions
                .Skip((PageNo - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }
    }
}

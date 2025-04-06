using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.Infrastructure.Paging.Country;
using Services;

namespace NordicBank.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICountryOverviewService _countryOverviewService;

        public IndexModel(ICountryOverviewService countryOverviewService)
        {
            _countryOverviewService = countryOverviewService;
        }


        public async Task OnGetAsync()
        {

        }
    }
}

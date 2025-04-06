using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.Infrastructure.Paging.Country;
using NordicBank.ViewModels;
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
        public List<CountryOverviewViewModel> CountryOverview { get; set; }

        public async Task OnGetAsync()
        {
            var dtoList = await _countryOverviewService.GetCountryOverviewAsync();
            CountryOverview = dtoList.Select(d => new CountryOverviewViewModel()
            {
                Country = d.Country,
                CountryCode = d.CountryCode,
                Clients = d.Clients,
                Accounts = d.Accounts,
                Capital = d.Capital,
            })
                .ToList();
        }
    }
}

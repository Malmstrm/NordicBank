using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
        public async Task<PartialViewResult> OnGetTopCustomersPartialAsync(string country)
        {
            var dtoList = await _countryOverviewService.GetTopCustomersByCountryAsync(country);

            var viewModels = dtoList.Select(d => new TopCustomerViewModel
            {
                CustomerId = d.CustomerId,
                Name = d.Name,
                City = d.City,
                TotalBalance = d.TotalBalance
            }).ToList();

            return new PartialViewResult
            {
                ViewName = "Partials/_TopCustomersPartial",
                ViewData = new ViewDataDictionary<List<TopCustomerViewModel>>(ViewData, viewModels)
            };
        }
    }
}

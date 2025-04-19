using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NordicBank.Infrastructure.Paging.Country;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.AML
{
    public class IndexModel : PageModel
    {
        private readonly IAntiMoneyLaunderingService _amlService;

        public IndexModel(IAntiMoneyLaunderingService amlService)
        {
            _amlService = amlService;
        }

        [BindProperty(SupportsGet = true)]
        public string SelectedCountry { get; set; } = "Sweden";

        public List<SelectListItem> CountryOptions { get; set; } = new();

        public List<ScanHistoryDTO> ScanLogs { get; set; } = new();
        public DateTime? LatestLogDate { get; set; }

        public async Task OnGetAsync()
        {
            SelectedCountry ??= "Sweden"; // Valfri default

            CountryOptions = CountryInfo.All
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name,
                    Selected = c.Name == SelectedCountry
                }).ToList();

            ScanLogs = await _amlService.GetScanHistoryAsync(SelectedCountry);

            if (ScanLogs.Any())
                LatestLogDate = ScanLogs.Max(log => log.CreatedAt);
        }
    }
}

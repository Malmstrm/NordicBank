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

        [BindProperty(SupportsGet = true)]
        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int CurrentPage => PageNo;

        public List<SelectListItem> CountryOptions { get; set; } = new();
        public List<ScanHistoryDTO> PagedScanLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            CountryOptions = CountryInfo.All
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name,
                    Selected = c.Name == SelectedCountry
                }).ToList();

            var logs = string.IsNullOrEmpty(SelectedCountry)
                ? new List<ScanHistoryDTO>()
                : await _amlService.GetScanHistoryAsync(SelectedCountry);

            TotalPages = (int)Math.Ceiling(logs.Count / (double)PageSize);
            PagedScanLogs = logs
                .Skip((PageNo - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}

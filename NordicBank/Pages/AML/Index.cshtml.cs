using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.AML
{
    public class IndexModel : PageModel
    {
        private readonly IAntiMoneyLaunderingService _scanService;

        public IndexModel(IAntiMoneyLaunderingService scanService)
        {
            _scanService = scanService;
        }

        [BindProperty]
        public string SelectedCountry { get; set; } = "Sweden";

        public List<string> Countries { get; } = new() { "Sweden", "Finland", "Denmark", "Norway" };
        public List<ScanHistoryDTO> ScanLogs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            ScanLogs = await _scanService.GetScanHistoryAsync(SelectedCountry);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedCountry))
                SelectedCountry = "Sweden";

            ScanLogs = await _scanService.GetScanHistoryAsync(SelectedCountry);
            return Page();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public List<ScanLogViewModel> ScanLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            var dtos = await _amlService.GetScanLogsAsync();
            ScanLogs = dtos.Select(d => new ScanLogViewModel
            {
                Id = d.Id,
                Country = d.Country,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                SuspiciousCount = d.SuspiciousCount,
                CreatedAt = d.CreatedAt
            }).ToList();
        }
    }
}

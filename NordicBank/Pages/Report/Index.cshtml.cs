using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace NordicBank.Pages.Report
{
    public class IndexModel : PageModel
    {
        private readonly ICountryOverviewService _service;

        public IndexModel(ICountryOverviewService service)
        {
            _service = service;
        }

        public List<CountryReportDTO> CountryReports { get; set; }

        public async Task OnGetAsync()
        {
            CountryReports = await _service.GetDetailedCountryReportAsync();
        }

    }
}
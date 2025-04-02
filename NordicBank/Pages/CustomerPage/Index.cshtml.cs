using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        public IndexModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public List<ViewCustomerViewModel> Customers { get; set; }
        public async Task OnGetAsync()
        {
            var dtos = await _customerService.GetViewAsync();

            Customers = dtos.Select(x => new ViewCustomerViewModel()
            {
                CustomerId = x.CustomerId,
                Givenname = x.Givenname,
                Streetaddress = x.Streetaddress,
                City = x.City,
                NationalId = x.NationalId,
                Status = x.Status,
            }).ToList();
        }
    }
}

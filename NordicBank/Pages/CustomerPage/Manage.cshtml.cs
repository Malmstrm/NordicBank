using AutoMapper;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.Infrastructure.Paging.Country;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class ManageModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public ManageModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }
        [BindProperty]
        public CustomerViewModel Customer { get; set; }
        public List<CountryInfo> Countries { get; set; }
        public bool IsEdit => Customer.CustomerId > 0;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Countries = CountryInfo.All;

            if (id == null) 
                Customer = new CustomerViewModel();
            else
            {
                var dto = await _customerService.GetByIdAsync(id.Value);
                if(dto == null) return NotFound();

                Customer = _mapper.Map<CustomerViewModel>(dto);
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Countries = CountryInfo.All;
                return Page();
            }

            var dto = _mapper.Map<CustomerDTO>(Customer);

            if (Customer.CustomerId == 0)
            {
                var created = await _customerService.CreateAsync(dto);
                return RedirectToPage("./Details", new { id = created.CustomerId });
            }
            else
            {
                var updated = await _customerService.UpdateAsync(dto);
                return RedirectToPage("./Details", new { id = updated.CustomerId });
            }
        }
    }
}

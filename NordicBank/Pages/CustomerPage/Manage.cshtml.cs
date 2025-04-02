using DataAccessLayer.DTO;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NordicBank.Infrastructure.Paging.Country;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class ManageModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public ManageModel(ICustomerService customerService)
        {
            _customerService = customerService;
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
                var dto = await _customerService.GetByIdAsyn(id.Value);
                if (dto == null) return NotFound();

                Customer = new CustomerViewModel
                {
                    CustomerId = dto.CustomerId,
                    Gender = dto.Gender,
                    Givenname = dto.Givenname,
                    Surname = dto.Surname,
                    Streetaddress = dto.Streetaddress,
                    City = dto.City,
                    Zipcode = dto.Zipcode,
                    Country = dto.Country,
                    CountryCode = dto.CountryCode,
                    Birthday = dto.Birthday,
                    NationalId = dto.NationalId,
                    Telephonecountrycode = dto.Telephonecountrycode,
                    Telephonenumber = dto.Telephonenumber,
                    Emailaddress = dto.Emailaddress,
                    Status = dto.Status
                };
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var dto = new CustomerDTO
            {
                CustomerId = Customer.CustomerId,
                Gender = Customer.Gender,
                Givenname = Customer.Givenname,
                Surname = Customer.Surname,
                Streetaddress = Customer.Streetaddress,
                City = Customer.City,
                Zipcode = Customer.Zipcode,
                Country = Customer.Country,
                CountryCode = Customer.CountryCode,
                Birthday = Customer.Birthday,
                NationalId = Customer.NationalId,
                Telephonecountrycode = Customer.Telephonecountrycode,
                Telephonenumber = Customer.Telephonenumber,
                Emailaddress = Customer.Emailaddress,
                Status = Customer.Status
            };

            if (Customer.CustomerId == 0)
            {
                // Create
                var created = await _customerService.CreateAsync(dto);
                return RedirectToPage("./Details", new { id = created.CustomerId });
            }
            else
            {
                // Edit
                var updated = await _customerService.UpdateAsync(dto);
                return RedirectToPage("./Details", new { id = updated.CustomerId });
            }
        }
    }
}

using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public DetailsModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public CustomerViewModel Customers { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _customerService.GetByIdAsyn(id);
            if (dto == null)
                return NotFound();

            Customers = new CustomerViewModel
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

            return Page();
        }
    }
}

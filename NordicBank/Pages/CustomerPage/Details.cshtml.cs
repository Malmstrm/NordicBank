using DataAccessLayer.Enums;
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
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _customerService.DeleteAsync(id);
            if(!success) return NotFound();

            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostActivateAsync(int id)
             => await ChangeStatus(id, CustomerStatus.Active);

        public async Task<IActionResult> OnPostDeactivateAsync(int id)
            => await ChangeStatus(id, CustomerStatus.Inactive);

        public async Task<IActionResult> OnPostBlacklistAsync(int id)
            => await ChangeStatus(id, CustomerStatus.Blacklisted);

        public async Task<IActionResult> OnPostDeceasedAsync(int id)
            => await ChangeStatus(id, CustomerStatus.Deceased);
        private async Task<IActionResult> ChangeStatus(int id, CustomerStatus newStatus)
        {
            var success = await _customerService.UpdateStatusAsync(id, newStatus);
            if(!success) return NotFound();

            return RedirectToPage(new { id });
        }
    }
}

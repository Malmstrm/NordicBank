using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        public DetailsModel(ICustomerService customerService, IAccountService accountService, ITransactionService transactionService)
        {
            _customerService = customerService;
            _accountService = accountService;
            _transactionService = transactionService;
        }
        public CustomerViewModel Customer { get; set; }
        public List<AccountViewModel> Account { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }

        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        [BindProperty(SupportsGet = true)] public int CustomerId { get; set; }
        [BindProperty] public string Frequency { get; set; } = "Monthly";

        public int TotalCount { get; set; }

        public string TotalBalance => Account.Sum(a => a.Balance).ToString("C");
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _customerService.GetByIdAsyn(id);
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

            var accountDtos = await _accountService.GetCustomerAccountAsync(dto.CustomerId);
            Account = accountDtos.Select(a => new AccountViewModel()
            {
                AccountId = a.AccountId,
                Frequency = a.Frequency,
                Created = a.Created,
                Balance = a.Balance,
                AccountStatus = a.AccountStatus,
                CustomerId = a.CustomerId,

            }).ToList();

            var recentTransactions = await _transactionService.GetLatestTransactionsCustomer(id);
            TotalCount = recentTransactions.Count;

            Transactions = recentTransactions
                .Select(t => new TransactionViewModel()
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Description = t.Description,
                    AccountId = t.AccountId
                })
                .ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var success = await _customerService.DeleteAsync(id);
            if(!success) return NotFound();

            return RedirectToPage("./Index");
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

            return RedirectToPage("Index");
        }
    }
}

using AutoMapper;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.CustomerPage
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public DetailsModel(ICustomerService customerService, IAccountService accountService, ITransactionService transactionService, IMapper mapper)
        {
            _customerService = customerService;
            _accountService = accountService;
            _transactionService = transactionService;
            _mapper = mapper;
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

            var dto = await _customerService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            Customer = _mapper.Map<CustomerViewModel>(dto);


            var accountDtos = await _accountService.GetCustomerAccountAsync(dto.CustomerId);
            Account = _mapper.Map<List<AccountViewModel>>(accountDtos);

            var recentTransactions = await _transactionService.GetLatestTransactionsCustomer(id);
            TotalCount = recentTransactions.Count;

            Transactions = _mapper.Map<List<TransactionViewModel>>(recentTransactions);

            return Page();
        }
        public async Task<IActionResult> OnPostCreateAccount()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Frequency))
            {
                await OnGetAsync(CustomerId); 
                ModelState.AddModelError("", "Please select a valid frequency.");
                return Page();
            }

            var accountId = await _accountService.CreateAccount(Id, Frequency);
            TempData["SuccessMessage"] = $"New account #{accountId} created!";
            return RedirectToPage(new { id = CustomerId });
        }
        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync(id);

            var success = await _customerService.UpdateStatusAsync(id, CustomerStatus.Active);
            if (!success) return NotFound();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync(id);

            var success = await _customerService.UpdateStatusAsync(id, CustomerStatus.Inactive);
            if (!success) return NotFound();

            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostBlacklistAsync(int id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync(id);

            var success = await _customerService.UpdateStatusAsync(id, CustomerStatus.Blacklisted);
            if (!success) return NotFound();

            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostDeceasedAsync(int id)
        {
            if (!ModelState.IsValid)
                return await OnGetAsync(id);

            var success = await _customerService.UpdateStatusAsync(id, CustomerStatus.Deceased);
            if (!success) return NotFound();

            return RedirectToPage("./Index");
        }
        public async Task<PartialViewResult> OnGetMoreInfoPartialAsync(int customerId)
        {
            var customer = await _customerService.GetByIdAsync(customerId); // använd din befintliga DTO

            var vm = _mapper.Map<CustomerDetailsViewModel>(customer);
            vm.NumberOfAccounts = await _accountService.GetAccountCountAsync(customerId);
            vm.TotalBalance = await _accountService.GetTotalBalanceAsync(customerId);

            return new PartialViewResult
            {
                ViewName = "Partials/_CustomerDetailsPartial",
                ViewData = new ViewDataDictionary<CustomerDetailsViewModel>(ViewData, vm)
            };
        }
    }
}

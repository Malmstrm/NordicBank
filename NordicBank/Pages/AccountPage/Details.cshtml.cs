using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;

namespace NordicBank.Pages.AccountPage
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        public DetailsModel(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }
        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        [BindProperty(SupportsGet = true)] public int CustomerId { get; set; }
        public int TotalCount { get; set; }

        public List<TransactionViewModel> Transactions { get; set; }
        public AccountViewModel Account {  get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var dto = await _accountService.GetAccountByIDAsync(Id);
            if(dto == null) return NotFound();

            Account = new AccountViewModel
            {
                AccountId = dto.AccountId,
                Frequency = dto.Frequency,
                Created = dto.Created,
                Balance = dto.Balance,
                AccountStatus = dto.AccountStatus,
                CustomerId = dto.CustomerId,

            };

            var allTransactions = await _transactionService.GetTransactionsIdAsync(Id);
            TotalCount = allTransactions.Count;

            Transactions = allTransactions
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Take(10)
                .Select(t => new TransactionViewModel()
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Description = t.Description,
                })
                .ToList();


            return Page();
        }
    }
}

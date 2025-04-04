using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using NordicBank.ViewModels;
using Services;
using System.ComponentModel.DataAnnotations;

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

        [BindProperty] public int ToAccount {  get; set; }
        [BindProperty] public int FromAccount { get; set; }

        [BindProperty][Required(ErrorMessage = "Amount is required.")] public int Amount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var success = await LoadAccountAndTransactionsAsync();
            return success ? Page() : NotFound();
        }
        public async Task<IActionResult> OnPostAsync(int id, decimal amount, string action)
        {
            if (!ModelState.IsValid) return Page();

            bool success = false;

            switch (action)
            {
                case "Deposit":
                    success = await _transactionService.DepositAsync(id, amount);
                    break;
                case "Withdraw":
                    success = await _transactionService.WithdrawAsync(id, amount);
                    break;
                default:
                    ModelState.AddModelError("", "Invalid actiom.");
                    break;
            }



            await LoadAccountAndTransactionsAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostTransferAsync(int id)
        {
            var amountStr = Request.Form["amount"];
            var toAccountIdStr = Request.Form["toAccountId"];

            if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
            {
                ModelState.AddModelError("", "Invalid amount.");
                await LoadAccountAndTransactionsAsync();
                return Page();
            }

            if (!int.TryParse(toAccountIdStr, out var toAccountId))
            {
                ModelState.AddModelError("", "Invalid destination account.");
                await LoadAccountAndTransactionsAsync();
                return Page();
            }

            if (id == toAccountId)
            {
                ModelState.AddModelError("", "You cannot transfer to same account.");
                await LoadAccountAndTransactionsAsync();
                return Page();
            }

            var success = await _transactionService.TransferAsync(id, toAccountId, amount);

            if (!success) ModelState.AddModelError("", "Transfer failed. Please check balance and accound info.");

            await LoadAccountAndTransactionsAsync();
            return Page();
        }
        private async Task<bool> LoadAccountAndTransactionsAsync()
        {
            var dto = await _accountService.GetAccountByIDAsync(Id);
            if (dto == null) return false;

            Account = new AccountViewModel
            {
                AccountId = dto.AccountId,
                Frequency = dto.Frequency,
                Created = dto.Created,
                Balance = dto.Balance,
                AccountStatus = dto.AccountStatus,
                CustomerId = dto.CustomerId
            };

            var allTransactions = await _transactionService.GetTransactionsIdAsync(Id);
            TotalCount = allTransactions.Count;

            Transactions = allTransactions
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Take(10)
                .Select(t => new TransactionViewModel
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Description = t.Description,
                })
                .ToList();

            return true;
        }
    }
}

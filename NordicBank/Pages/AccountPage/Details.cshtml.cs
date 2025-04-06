using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NordicBank.ViewModels;
using Services;
using Services.Utility;
using System.Text;

namespace NordicBank.Pages.AccountPage;

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

    public AccountViewModel Account { get; set; } = null!;
    public List<TransactionViewModel> Transactions { get; set; } = new();
    public int TotalCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var success = await LoadAccountAndTransactionsAsync();
        return success ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostActivateAsync(int id)
    {
        var success = await _accountService.UpdateStatusAsync(id, AccountStatus.Active);
        if (!success) return NotFound();

        await LoadAccountAndTransactionsAsync();
        return RedirectToPage("/CustomerPage/Details", new { id = CustomerId });
    }

    public async Task<IActionResult> OnPostDeactivateAsync(int id)
    {
        var success = await _accountService.UpdateStatusAsync(id, AccountStatus.Inactive);
        if (!success) return NotFound();

        await LoadAccountAndTransactionsAsync();
        return RedirectToPage("/CustomerPage/Details", new { id = CustomerId });
    }

    public async Task<IActionResult> OnPostAsync(int id, decimal amount, [FromForm] string action)
    {
        if (!ModelState.IsValid) return Page();

        TransactionResult result = action switch
        {
            "Deposit" => await _transactionService.DepositAsync(id, amount),
            "Withdraw" => await _transactionService.WithdrawAsync(id, amount),
            _ => TransactionResult.Failed("Invalid action.")
        };

        if (result.Success)
            TempData["SuccessMessage"] = $"{action} of {amount:C} completed successfully.";
        else
            ModelState.AddModelError("", result.Message ?? "Transaction failed");

        return await ReloadAndReturn();
    }

    public async Task<IActionResult> OnPostTransferAsync(int id)
    {
        if (!ModelState.IsValid) return await ReloadAndReturn();

        var amountStr = Request.Form["amount"];
        var toAccountIdStr = Request.Form["toAccountId"];

        if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
            ModelState.AddModelError("", "Invalid amount.");
        else if (!int.TryParse(toAccountIdStr, out var toAccountId))
            ModelState.AddModelError("", "Invalid target account.");
        else if (id == toAccountId)
            ModelState.AddModelError("", "You cannot transfer to the same account");
        else
        {
            var result = await _transactionService.TransferAsync(id, toAccountId, amount);

            if(!result.Success)
                ModelState.AddModelError("", result.Message ?? "Transfer failed. Check balance and status.");
            else
            {
                TempData["SuccessMessage"] = result.Message ?? $"Transferred {amount:C} to account {toAccountId}.";
            }
        }
        return await ReloadAndReturn();
    }
    public async Task<IActionResult> OnGetFetchTransactionsAsync(int accountId, int skip)
    {
        var nextTransactions = await _transactionService.GetTransactionsPagedAsync(accountId, skip, 20);

        if (!nextTransactions.Any())
            return Content("");

        var htmlBuilder = new StringBuilder();

        foreach (var t in nextTransactions)
        {
            var vm = new TransactionViewModel
            {
                Date = t.Date,
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Balance = t.Balance,
                Description = t.Description
            };

            var rendered = await RazorPartialToString.RenderPartialViewToString(this, "_TransactionRow", vm);
            htmlBuilder.AppendLine(rendered);
        }

        return Content(htmlBuilder.ToString(), "text/html");
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
                Description = t.Description
            })
            .ToList();

        return true;
    }
    private async Task<IActionResult> ReloadAndReturn()
    {
        await LoadAccountAndTransactionsAsync();
        return Page();
    }
}

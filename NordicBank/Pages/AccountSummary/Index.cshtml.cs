using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using NordicBank.ViewModels;

namespace NordicBank.Pages.AccountSummary
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public List<AccountSummaryViewModel> Accounts { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string SortColumn { get; set; } = "AccountId";
        public string SortOrder { get; set; } = "asc";
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, string? searchTerm, int pageNo = 1)
        {
            SortColumn = sortColumn ?? "AccountId";
            SortOrder = sortOrder ?? "asc";
            CurrentPage = pageNo;

            var dtos = await _accountService.GetAccountSummaryListAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dtos = dtos
                    .Where(a => a.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            dtos = (SortColumn, SortOrder) switch
            {
                ("CustomerName", "asc") => dtos.OrderBy(a => a.CustomerName).ToList(),
                ("CustomerName", "desc") => dtos.OrderByDescending(a => a.CustomerName).ToList(),
                ("Balance", "asc") => dtos.OrderBy(a => a.Balance).ToList(),
                ("Balance", "desc") => dtos.OrderByDescending(a => a.Balance).ToList(),
                ("AccountId", "asc") => dtos.OrderBy(a => a.AccountId).ToList(),
                ("AccountId", "desc") => dtos.OrderByDescending(a => a.AccountId).ToList(),
                _ => dtos
            };

            TotalPages = (int)Math.Ceiling(dtos.Count / (double)PageSize);

            Accounts = dtos
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(dto => new AccountSummaryViewModel
                {
                    AccountId = dto.AccountId,
                    CustomerName = dto.CustomerName,
                    Created = dto.Created,
                    Balance = dto.Balance,
                    Frequency = dto.Frequency,
                    AccountStatus = dto.AccountStatus
                })
                .ToList();
        }
    }

}

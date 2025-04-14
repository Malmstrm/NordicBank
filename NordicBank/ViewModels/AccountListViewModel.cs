using DataAccessLayer.DTO;
using NordicBank.Infrastructure.Paging;

namespace NordicBank.ViewModels
{
    public class AccountListViewModel : PagedResultBase
    {
        public List<AccountSummaryDTO> Accounts { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }


    }
}

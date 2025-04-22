using DataAccessLayer.DTO;

namespace NordicBank.ViewModels
{
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SortOrder { get; set; }
    }
}

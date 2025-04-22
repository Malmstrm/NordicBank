using DataAccessLayer.DTO;

namespace NordicBank.ViewModels
{
    public class UserListViewModel
    {
        public List<UserDTO> Users { get; set; } = new();
        public string? SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

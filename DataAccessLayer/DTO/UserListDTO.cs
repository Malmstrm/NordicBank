namespace DataAccessLayer.DTO
{
    public class UserListDTO
    {
        public List<UserDTO> Users { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SortOrder { get; set; }
    }
}

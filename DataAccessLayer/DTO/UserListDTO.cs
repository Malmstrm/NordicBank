namespace DataAccessLayer.DTO
{
    public class UserListDTO
    {
        public List<UserDTO> Users { get; set; } = new();
        public string? CurrentSort { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

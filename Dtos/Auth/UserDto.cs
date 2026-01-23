namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserDto
    {
        public string Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Role { get; set; } 
    }
}

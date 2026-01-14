namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserUpdateDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public DateTime DateofBirth { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}

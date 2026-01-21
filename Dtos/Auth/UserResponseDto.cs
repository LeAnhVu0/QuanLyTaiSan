namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }
        public int? DepartmentId { get; set; }
    }
}

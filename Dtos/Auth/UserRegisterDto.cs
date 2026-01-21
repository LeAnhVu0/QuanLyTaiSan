using QuanLyTaiSan.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserRegisterDto
    {
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        public required string Password { get; set; }
        public DateTime DateofBirth { get; set; }
        public string? Address { get; set; }
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
      
        public int? DepartmentId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

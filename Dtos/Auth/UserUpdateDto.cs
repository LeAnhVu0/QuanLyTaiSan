using QuanLyTaiSan.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserUpdateDto
    {
        public required string Username { get; set; }
        public DateTime DateofBirth { get; set; }
        public string? FullName { get; set; }
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Email { get; set; }
     
    }
}

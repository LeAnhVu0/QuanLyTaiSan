using QuanLyTaiSan.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.Auth
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Username không được để trống")]
        public required string Username { get; set; }
        public string? Fullname { get; set; }
        [Required(ErrorMessage = "password không được để trống")]
        public required string Password { get; set; }
        public DateTime DateofBirth { get; set; }
        public string? Address { get; set; }
        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "email không được để trống")]
        public required string Email { get; set; }
      
        public int? DepartmentId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

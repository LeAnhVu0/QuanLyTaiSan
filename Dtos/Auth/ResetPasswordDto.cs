using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }


        [MaxLength(200)]
        public string? DeviceInfo { get; set; }

        [MaxLength(50)]

        public ApplicationUser User { get; set; } = null!;
    }
}

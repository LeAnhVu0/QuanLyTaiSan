using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Models
{
    public class AssetHistory
    {
        [Key]
        public int HistoryID { get; set; }

        [StringLength(255)]
        public string ActionType { get; set; } = string.Empty; 

        [StringLength(255)]
        public string? Descriptions { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;

        public string? AssetName { get; set; }
        public decimal OriginalValue { get; set; }
        public int Status { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        // Khóa ngoại
        public int AssetId { get; set; } 
        public int? UserId { get; set; } 

        public Asset? Asset { get; set; }
    }
}

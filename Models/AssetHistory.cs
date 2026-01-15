using QuanLyTaiSan.Models;
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
        public int Status { get; set; }

        // Khóa ngoại
        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedToUser {  get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser CreatedByUser { get; set; } 
        public int AssetId { get; set; } 
        public Asset? Asset { get; set; }


    }
}

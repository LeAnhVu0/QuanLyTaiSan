using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Dtos.AssetHistory
{
    public class AssetHistoryDto
    {

        public int HistoryID { get; set; }

        public string? ActionType { get; set; }

        public string? Descriptions { get; set; }

        public DateTime ActionDate { get; set; }

        public string? AssetName { get; set; }
        public decimal OriginalValue { get; set; }
        public int Status { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        // Khóa ngoại
        public int AssetId { get; set; }
        public int? UserId { get; set; }

    }
}

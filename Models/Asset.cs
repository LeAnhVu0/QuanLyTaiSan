using System.ComponentModel.DataAnnotations;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Enum;
namespace QuanLyTaiSanTest.Models
{
    public class Asset
    {
        public int AssetId { get; set; }

        [StringLength(10)]
        public required string AssetCode { get; set; }

        [StringLength(50)]
        public required string AssetName { get; set; }

        public decimal OriginalValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? ManufactureYear { get; set; }
        [StringLength(255)]
        public string? Descriptions { get; set; }

        [StringLength(10)]
        public string? Unit { get; set; }

        public AssetStatus Status { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }


        [StringLength(255)]
        public string? ImageAsset { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        
        //khóa ngoài
        public int CategoryId {  get; set; }
        public Category? Category { get; set; }

        // kết nối user và department qua id
        public string? UserId { get; set; } 
        public ApplicationUser? User { get; set; }

        public int DepartmentId { get; set; } 
        public Department? Department { get; set; }
    }
}

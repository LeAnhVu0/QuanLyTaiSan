using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Dtos.Asset
{
    public class AssetRespondDto
    {
        public int AssetId { get; set; }

        public required string AssetCode { get; set; }

        public required string AssetName { get; set; }

        public decimal OriginalValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? ManufactureYear { get; set; }
        public string? Descriptions { get; set; }

        public string? Unit { get; set; }

        public string? Status { get; set; }

        public string? Note { get; set; }

        public string? ImageAsset { get; set; }
        public DateTime CreatedTime { get; set; } 
        public DateTime? UpdatedTime { get; set; }
        public int CategoryId {  get; set; }
        public int DepartmentId { get; set; }
        public string? UserId { get; set; }
        public UserDto User { get; set; }

    }
}

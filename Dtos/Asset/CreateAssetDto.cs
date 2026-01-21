using QuanLyTaiSanTest.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Dtos.Asset
{
    public class CreateAssetDto
    {

        [StringLength(50, MinimumLength = 3)]
        public required string AssetName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị tài sản không được là số âm")]
        public decimal OriginalValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? ManufactureYear { get; set; }
        public string? Descriptions { get; set; }

        public string? Unit { get; set; }

        public string? Note { get; set; }

        public string? ImageAsset { get; set; }
        public required int CategoryId { get; set; }
        public required int DepartmentId { get; set; }
    }
}

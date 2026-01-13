using QuanLyTaiSanTest.Enum;

namespace QuanLyTaiSanTest.Dtos.Asset
{
    public class CreateAssetDto
    {


        public required string AssetName { get; set; }

        public decimal OriginalValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? ManufactureYear { get; set; }
        public string? Descriptions { get; set; }

        public string? Unit { get; set; }

        public string? Note { get; set; }

        public string? ImageAsset { get; set; }
        public int CategoryId { get; set; }
        public int DepartmentId { get; set; }
    }
}

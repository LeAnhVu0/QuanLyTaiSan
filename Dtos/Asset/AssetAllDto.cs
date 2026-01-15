using QuanLyTaiSanTest.Dtos.Asset;

namespace QuanLyTaiSan.Dtos.Asset
{
    public class AssetAllDto
    {
        public List<AssetDto> ListAsset { get; set; } = new List<AssetDto>();

        public string? SearchName { get; set; }
        public int? categoryId { get; set; }
        public int? Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.AssetHistory;

namespace QuanLyTaiSan.Dtos.AssetHistory
{
    public class AssetHistoryAllDto
    {
        public List<AssetHistoryDto> ListAssetHistory { get; set; } = new List<AssetHistoryDto>();

        public string? SearchName { get; set; }
        public string? ActionType { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

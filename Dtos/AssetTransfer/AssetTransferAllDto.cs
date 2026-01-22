using QuanLyTaiSanTest.Dtos.Asset;

namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class AssetTransferAllDto
    {
        public List<AssetTransferResponseDto> ListAsset { get; set; } = new List<AssetTransferResponseDto>();

        public int? Type { get; set; }
        public int? Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

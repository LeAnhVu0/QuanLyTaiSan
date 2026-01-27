using QuanLyTaiSanTest.Dtos.Asset;

namespace QuanLyTaiSan.Dtos.Inventory
{
    public class InventoryAllDto
    {
        public List<InventoryResponseDto> ListInventory { get; set; } = new List<InventoryResponseDto>();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

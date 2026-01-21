using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Category;

namespace QuanLyTaiSan.Dtos.Category
{
    public class CategoryAllDtocs
    {
        public List<CategoryResponseDto> ListAsset { get; set; } = new List<CategoryResponseDto>();

        public string? SearchName { get; set; }
        public int? Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

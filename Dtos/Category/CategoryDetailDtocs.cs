using QuanLyTaiSan.Dtos.Asset;

namespace QuanLyTaiSan.Dtos.Category
{
    public class CategoryDetailDtocs
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public List<AssetNameDtp> Assets { get; set; } = new List<AssetNameDtp>();
    }
}

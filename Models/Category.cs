using QuanLyTaiSanTest.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [StringLength(50)]
        public required string CategoryName { get; set; }
        public string? Description {  get; set; }
        public CategoryStatus Status { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
        public ICollection<Asset> Assets { get; set; }=new List<Asset>();
        public bool IsDelete { get; set; } = false;
    }
}

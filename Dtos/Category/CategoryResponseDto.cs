using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Dtos.Category
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedTime { get; set; } 
        public DateTime? UpdatedTime { get; set; }
        public List<int> AssetIds { get; set; } = new List<int>(); 
    }
}

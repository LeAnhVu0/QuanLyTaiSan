using QuanLyTaiSan.Enum;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSan.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public DepartmentStatus DepartmentStatus { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public ICollection<ApplicationUser> User { get; set; } = new List<ApplicationUser>();
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}

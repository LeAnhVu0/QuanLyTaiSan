using Microsoft.AspNetCore.Identity;

namespace QuanLyTaiSan.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}

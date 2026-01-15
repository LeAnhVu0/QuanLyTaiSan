    using Microsoft.AspNetCore.Identity;
    using QuanLyTaiSanTest.Models;

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

            public ICollection<Asset> Assets { get; set; } = new List<Asset>();
            public ICollection<Inventory> inventories { get; set; } = new List<Inventory>();
    }
    }

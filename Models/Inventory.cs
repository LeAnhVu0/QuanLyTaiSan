using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSanTest.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime InventoryDate { get; set; }
        public int BookQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public string? Note { get; set; }
        public InventoryStatus Status { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get;set; }

        public string UserIdBy { get; set; }
        public ApplicationUser User { get; set; }
    }
}

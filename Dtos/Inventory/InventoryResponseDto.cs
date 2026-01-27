using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Dtos.Inventory
{
    public class InventoryResponseDto
    {
        public int InventoryId { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime InventoryDate { get; set; }
        public int BookQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public string Note { get; set; }
        public DepartmentDto? Department { get; set; }

        public UserDto? User { get; set; }
        public string Status {  get; set; }
    }
}

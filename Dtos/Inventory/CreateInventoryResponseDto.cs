namespace QuanLyTaiSan.Dtos.Inventory
{
    public class CreateInventoryResponseDto
    {
        public int InventoryId { get; set; }
        public DateTime PlanDate { get; set; }
        public int DepartmentId { get; set; }
        public string UserIdBy { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }


    }
}

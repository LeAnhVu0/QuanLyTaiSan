namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
    }
}

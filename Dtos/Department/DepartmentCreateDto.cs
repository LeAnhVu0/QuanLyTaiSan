namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentCreateDto
    {
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public string? DepartmentStatus { get; set; }
    }
}

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentUpdateDto
    {
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public string? DepartmentStatus { get; set; }
    }
}

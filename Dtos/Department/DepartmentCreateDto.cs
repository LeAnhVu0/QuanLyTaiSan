using QuanLyTaiSan.Enum;

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentCreateDto
    {
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public DepartmentStatus? DepartmentStatus { get; set; }
    }
}

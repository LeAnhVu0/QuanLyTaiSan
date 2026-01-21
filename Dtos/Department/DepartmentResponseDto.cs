using QuanLyTaiSan.Enum;

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentResponseDto
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public DepartmentStatus? DepartmentStatus { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
    }
}

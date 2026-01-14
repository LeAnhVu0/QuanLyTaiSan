using QuanLyTaiSan.Dtos.Auth;

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public string? DepartmentStatus { get; set; }
        public List<UserInDepartmentDto> Users { get; set; }
    }
}

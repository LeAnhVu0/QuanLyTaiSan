using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Enum;

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        public DepartmentStatus? DepartmentStatus { get; set; }
        public List<UserInDepartmentDto> Users { get; set; }
        public List<AssetNameDto> Assets { get; set; }
    }
}

using QuanLyTaiSan.Enum;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.Department
{
    public class DepartmentUpdateDto
    {
        public required string DepartmentName { get; set; }

        public string? Description { get; set; }
        [EnumDataType(typeof(DepartmentStatus))]
        public DepartmentStatus DepartmentStatus { get; set; }
    }
}

using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResult<DepartmentResponseDto>> GetDepartmentsAsync(int pageIndex, int pageSize);
        Task<DepartmentDetailDto> GetDepartmentById(int id);
        Task<DepartmentResponseDto> AddDepartment(DepartmentCreateDto department);
        Task<DepartmentResponseDto> UpdateDepartment(int id, DepartmentUpdateDto department);
        Task<string> DeleteDepartment(int id);
    }
}

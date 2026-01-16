using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResult<DepartmentDto>> GetDepartmentsAsync(int pageIndex, int pageSize);
        Task<DepartmentDetailDto> GetDepartmentById(int id);
        Task<DepartmentDto> AddDepartment(DepartmentCreateDto department);
        Task<DepartmentDto> UpdateDepartment(int id, DepartmentUpdateDto department);
        Task<string> DeleteDepartment(int id);
    }
}

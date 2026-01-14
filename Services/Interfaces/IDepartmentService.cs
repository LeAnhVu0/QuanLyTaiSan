using QuanLyTaiSan.Dtos.Department;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAll();
        Task<DepartmentDetailDto> GetDepartmentById(int id);
        Task<DepartmentDto> AddDepartment(DepartmentCreateDto department);
        Task<DepartmentDto> UpdateDepartment(int id, DepartmentUpdateDto department);
        Task<string> DeleteDepartment(int id);
    }
}

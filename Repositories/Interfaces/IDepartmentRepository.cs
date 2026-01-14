using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAll();
        Task<Department?> GetDepartmentById(int id);
        Task<Department> AddDepartment(Department department);
        Task<Department> UpdateDepartment(Department department);
        Task DeleteDepartment(Department department);
        Task SaveAsync();
    }
}

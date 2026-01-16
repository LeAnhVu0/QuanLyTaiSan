using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Interfaces;
using QuanLyTaiSanTest.Data;
using Microsoft.EntityFrameworkCore;
namespace QuanLyTaiSan.Repositories.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _appDbContext;
        public DepartmentRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IQueryable<Department> GetAll()
        {
            return _appDbContext.Department.AsQueryable();
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            return await _appDbContext.Department
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> AddDepartment(Department department)
        {
            var result = await _appDbContext.Department.AddAsync(department);

            return result.Entity;

        }
        public async Task<Department> UpdateDepartment(Department department)
        {
            _appDbContext.Department.Update(department);
            return department;
        }
        public Task DeleteDepartment(Department department)
        {
            _appDbContext.Department.Remove(department);
            return Task.CompletedTask;
        }
        public async Task SaveAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}

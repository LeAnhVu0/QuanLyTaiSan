using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Interfaces;
using QuanLyTaiSan.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace QuanLyTaiSan.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<PagedResult<DepartmentDto>> GetDepartmentsAsync(int pageIndex, int pageSize)
        {
            var totalCount =await _repository.GetAll().CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await _repository.GetAll()
           .Skip((pageIndex - 1) * pageSize)
           .Take(pageSize)
           .Select(d => new DepartmentDto
           {
               Id = d.Id,
               DepartmentName = d.DepartmentName,
               Description = d.Description,
               DepartmentStatus = d.DepartmentStatus
           })
           .ToListAsync();

            return new PagedResult<DepartmentDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPage = totalPage,
                Items = items
            };
        }
        public async Task<DepartmentDetailDto> GetDepartmentById(int id)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;
            return new DepartmentDetailDto
            {
                Id = department.Id,
                DepartmentName = department.DepartmentName,
                Users = department.User.Select(u => new UserInDepartmentDto
                {
                    Id = u.Id,
                    FullName = u.UserName,
                    Email = u.Email
                }).ToList()
            };
        }
        public async Task<DepartmentDto> AddDepartment(DepartmentCreateDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            await _repository.AddDepartment(department);
            await _repository.SaveAsync();
            return _mapper.Map<DepartmentDto>(department);
        }
        public async Task<DepartmentDto> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;
            _mapper.Map(dto, department);
            await _repository.SaveAsync();
            return _mapper.Map<DepartmentDto>(department);

        }
        public async Task<string> DeleteDepartment(int id)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;
            _repository.DeleteDepartment(department);
            await _repository.SaveAsync();
            return $"{department.Id} deleted";
        }
    }
}

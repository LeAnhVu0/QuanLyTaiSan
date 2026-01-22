using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<PagedResult<DepartmentResponseDto>> GetDepartmentsAsync(int pageIndex, int pageSize)
        {
            var totalCount =await _repository.GetAll().CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await _repository.GetAll()
           .Skip((pageIndex - 1) * pageSize)
           .Take(pageSize)
           .Select(d => new DepartmentResponseDto
           {
               Id = d.Id,
               DepartmentName = d.DepartmentName,
               Description = d.Description,
               DepartmentStatus = d.DepartmentStatus
           })
           .ToListAsync();

            return new PagedResult<DepartmentResponseDto>
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
                    FullName = u.FullName,
                    Email = u.Email
                }).ToList()
            };
        }
        public async Task<DepartmentResponseDto> AddDepartment(DepartmentCreateDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            await _repository.AddDepartment(department);
            await _repository.SaveAsync();
            return _mapper.Map<DepartmentResponseDto>(department);
        }
        public async Task<DepartmentResponseDto> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;
            _mapper.Map(dto, department);
            await _repository.SaveAsync();
            return _mapper.Map<DepartmentResponseDto>(department);

        }
        public async Task<string> DeleteDepartment(int id)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;
            var hasUser = await _userManager.Users
     .AnyAsync(u => u.DepartmentId == department.Id);

            if (hasUser)
            {
                return "Cannot delete department with assigned users";
            }
            _repository.DeleteDepartment(department);
            await _repository.SaveAsync();
            return $"{department.Id} deleted";
        }
    }
}

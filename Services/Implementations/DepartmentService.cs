using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Interfaces;
using QuanLyTaiSan.Services.Interfaces;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace QuanLyTaiSan.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAssetRepository _assetRepository;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper, UserManager<ApplicationUser> userManager,IAssetRepository assetRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _userManager = userManager;
            _assetRepository = assetRepository;
        }
        public async Task<PagedResult<DepartmentResponseDto>> GetDepartmentsAsync(int pageIndex, int pageSize)
        {
            var totalCount =await _repository.GetAll().CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await _repository.GetAll()
           .OrderByDescending(d => d.CreateTime)
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
                Description=department.Description,
                DepartmentStatus=department.DepartmentStatus,
                Users = department.User.Select(u => new UserInDepartmentDto
                {
                    Id = u.Id,
                    UseName = u.UserName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Status = u.Status.ToFriendlyString()
                }).ToList(),
                Assets=department.Assets.Select(a=>new AssetNameDto
                {
                    AssetId=a.AssetId,
                    AssetCode=a.AssetCode,
                    AssetName=a.AssetName,
                    AssetStatus = a.Status.ToDisplayName()
                }).ToList()
            };
        }
        public async Task<DepartmentResponseDto> AddDepartment(DepartmentCreateDto dto)
        {
            
            var departmentName = dto.DepartmentName.Trim();

            var exists = await _repository.GetAll()
                .AnyAsync(d => d.DepartmentName == departmentName);

            if (exists)
                throw new InvalidOperationException("Phòng ban đã tồn tại");

            var department = _mapper.Map<Department>(dto);
            department.DepartmentName = departmentName;

            await _repository.AddDepartment(department);
            await _repository.SaveAsync();

            return _mapper.Map<DepartmentResponseDto>(department);
        }


        public async Task<DepartmentResponseDto> UpdateDepartment(
 int id, DepartmentUpdateDto dto)
        {
            var department = await _repository.GetDepartmentById(id);
            if (department == null)
                return null;

            // ❌ Không cho set status = Deleted
            if (dto.DepartmentStatus == DepartmentStatus.Deleted)
                throw new InvalidOperationException("Không thể cập nhật trạng thái Deleted");

            // 👉 CHỈ kiểm tra khi có ý định đổi status
            if (dto.DepartmentStatus != department.DepartmentStatus)
            {
                // Ví dụ: đổi từ Active → Inactive
                if (dto.DepartmentStatus == DepartmentStatus.Inactive)
                {
                    var hasUser = await _userManager.Users
                        .AnyAsync(u => u.DepartmentId == department.Id);

                    var hasAsset = await _assetRepository
                        .AnyAssetAsync(a => a.DepartmentId == department.Id);

                    if (hasUser || hasAsset)
                        throw new InvalidOperationException(
                            "Không thể ngừng sử dụng phòng ban đang có nhân viên hoặc tài sản");
                }

                department.DepartmentStatus = dto.DepartmentStatus;
            }

            // ✅ update các field khác bình thường
            department.DepartmentName = dto.DepartmentName;
            department.Description = dto.Description;

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

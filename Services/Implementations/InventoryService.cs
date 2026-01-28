using Microsoft.AspNetCore.Http;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Inventory;
using QuanLyTaiSan.Enum;
using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssetRepository _repoAsset;

        public InventoryService(IInventoryRepository repo, IHttpContextAccessor httpContextAccessor , IAssetRepository repoAsset) 
        {
            _repo=repo;
            _httpContextAccessor = httpContextAccessor;
            _repoAsset = repoAsset;
        }
        public async Task<CreateInventoryResponseDto> CreatePlan(CreateInventoryDto createInventoryDto)
        {
            var inventory = new Inventory
            {
                PlanDate = createInventoryDto.PlanDate,
                DepartmentId = createInventoryDto.DepartmentId,
                Note = createInventoryDto.Note,
                UserIdBy = _httpContextAccessor.HttpContext?.User?
                                               .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value,
                Status = QuanLyTaiSan.Enum.InventoryStatus.ChuaKiemKe
            } ;
           var result =  await _repo.Create(inventory);
           return new CreateInventoryResponseDto
            {
                InventoryId = result.InventoryId,
                PlanDate = result.PlanDate,
                DepartmentId = result.DepartmentId,
                UserIdBy = result.UserIdBy,
                Note = result.Note,
                Status = result.Status.ToDisplayName()
            };
        }

        public async Task<InventoryAllDto> GetAll(int pageIndex, int pageSize, int? departmentId, int? status)
        {
            var data = await _repo.GetAll( pageIndex, pageSize, departmentId, status);
            if (data.Items == null || data.Items.Count == 0)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }
            var items =  data.Items.Select(h => new InventoryResponseDto
            {
                InventoryId = h.InventoryId,
                PlanDate = h.PlanDate,
                InventoryDate = h.InventoryDate,
                BookQuantity = h.BookQuantity,
                ActualQuantity = h.ActualQuantity,
                Note = h.Note,
                Department = new QuanLyTaiSan.Dtos.Department.DepartmentDto
                { 
                    Id = h.DepartmentId,
                    DepartmentName = h.Department.DepartmentName,
                    Description = h.Department.Description
                },
                User = new QuanLyTaiSan.Dtos.Auth.UserDto
                { 
                    Id = h.User.Id,
                    Username = h.User.UserName,
                    Fullname = h.User.FullName 
                },
                Status = h.Status.ToDisplayName()
            }).ToList();
            var totalPage = (int)Math.Ceiling(data.TotalCount / (double)pageSize);

            return new InventoryAllDto
            {
                ListInventory = items,
                
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = data.TotalCount,
                TotalPage = totalPage,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPage
            };

        }
        public async Task<InventoryResponseDto> GetById(int id)
        {
            var result = await _repo.GetById(id);
            if(result == null)
                throw new KeyNotFoundException("Phiếu không tồn tại");

            return new InventoryResponseDto
            {
                InventoryId = result.InventoryId,
                PlanDate = result.PlanDate,
                Department = new QuanLyTaiSan.Dtos.Department.DepartmentDto
                {
                    Id = result.DepartmentId,
                    DepartmentName = result.Department.DepartmentName,
                    Description = result.Department.Description
                },
                User = new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = result.UserIdBy,
                    Username = result.User.UserName,
                    Fullname = result.User.FullName,

                },
                Note = result.Note,
                InventoryDate = result.InventoryDate,
                ActualQuantity = result.ActualQuantity,
                BookQuantity = result.BookQuantity,
                Status = result.Status.ToDisplayName()
            };
        
        }

        public async Task<InventoryResponseDto> Update(int id, UpdateInventoryDto dto)
        {
            var result = await _repo.GetById(id);

            if (result == null)
            {
                throw new KeyNotFoundException("Không có phiếu kiểm kê");
            }    
            result.InventoryDate = dto.InventoryDate;
            result.BookQuantity = await _repoAsset.CountAssetsByDepartment(result.DepartmentId);
            result.ActualQuantity = dto.ActualQuantity;
            result.Status = (result.ActualQuantity != result.BookQuantity) ? InventoryStatus.ChenhLech : InventoryStatus.KhopSoLuong;
            await _repo.Update();
            return new InventoryResponseDto
            {
                InventoryId = result.InventoryId,
                PlanDate = result.PlanDate,
                Department = new QuanLyTaiSan.Dtos.Department.DepartmentDto
                {
                    Id = result.DepartmentId,
                    DepartmentName = result.Department.DepartmentName,
                    Description = result.Department.Description
                },
                User = new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = result.UserIdBy,
                    Username = result.User.UserName,
                    Fullname = result.User.FullName,

                },
                Note = result.Note,
                InventoryDate = result.InventoryDate,
                ActualQuantity = result.ActualQuantity,
                BookQuantity = result.BookQuantity,
                Status = (result.ActualQuantity != result.BookQuantity) ? InventoryStatus.ChenhLech.ToDisplayName() : InventoryStatus.KhopSoLuong.ToDisplayName()
            };
        }
    }
}

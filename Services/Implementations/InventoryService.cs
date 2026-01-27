using Microsoft.AspNetCore.Http;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Inventory;
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

        public InventoryService(IInventoryRepository repo, IHttpContextAccessor httpContextAccessor) 
        {
            _repo=repo;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Inventory> CreatePlan(CreateInventoryDto createInventoryDto)
        {
            var inventory = new Inventory
            {
                PlanDate = createInventoryDto.PlanDate,
                DepartmentId = createInventoryDto.DepartmentId,
                Note = createInventoryDto.Note,
                UserIdBy = _httpContextAccessor.HttpContext?.User?
                                               .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
            } ;
           return await _repo.Create(inventory);
        }

        public async Task<InventoryAllDto> GetAll(int pageIndex, int pageSize)
        {
            var data = await _repo.GetAll( pageIndex, pageSize);
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
                }
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

        public async Task Update(int id, UpdateInventoryDto dto)
        {
            var inv = await _repo.GetById(id);

            if (inv == null)
            {
                throw new KeyNotFoundException("Không có phiếu kiểm kê");
            }    
            inv.InventoryDate = dto.InventoryDate;
            inv.BookQuantity = dto.BookQuantity;
            inv.ActualQuantity = dto.ActualQuantity;
            await _repo.Update();
        }
    }
}

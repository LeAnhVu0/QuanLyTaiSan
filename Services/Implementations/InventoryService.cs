using Microsoft.AspNetCore.Http;
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

        public async Task<List<InventoryResponseDto>> GetAll()
        {
            var list = await _repo.GetAll();
            return list.Select(h => new InventoryResponseDto
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
                    //Email = h.User.Email,
                    //PhoneNumber = h.User.PhoneNumber
                }

            }).ToList();
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

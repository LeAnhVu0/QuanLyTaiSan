using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;

        public InventoryService(IInventoryRepository repo) 
        {
            _repo=repo;
        }
        public async Task<Inventory> CreatePlan(CreateInventoryDto createInventoryDto)
        {
            var inventory = new Inventory
            {
                PlanDate = createInventoryDto.PlanDate,
                DepartmentId = createInventoryDto.DepartmentId,
                Note = createInventoryDto.Note,
                UserIdBy = createInventoryDto.UserIdBy
            } ;
           return await _repo.Create(inventory);
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

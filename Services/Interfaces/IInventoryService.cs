using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IInventoryService
    {
        public Task<Inventory> CreatePlan(CreateInventoryDto createInventoryDto);
        public Task Update(int id , UpdateInventoryDto updateInventoryDto);
    }
}

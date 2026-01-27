using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Inventory;
using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IInventoryService
    {
        public Task<CreateInventoryResponseDto> CreatePlan(CreateInventoryDto createInventoryDto);
        public Task<InventoryResponseDto> Update(int id , UpdateInventoryDto updateInventoryDto);
        public Task<InventoryAllDto> GetAll(int pageIndex, int pageSize, int? departmentId, int? status);
        public Task<InventoryResponseDto> GetById(int id);

    }
}

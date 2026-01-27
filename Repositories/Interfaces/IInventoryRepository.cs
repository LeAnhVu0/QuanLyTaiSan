using QuanLyTaiSan.Dtos.Inventory;
using QuanLyTaiSanTest.Models;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
       public Task<Inventory> Create(Inventory inventory);
       public Task<Inventory?> GetById(int id);
       public Task Update();
       public Task<(List<Inventory> Items, int TotalCount)> GetAll(int pageIndex, int pageSize, int? departmentId, int? status);

    }
}

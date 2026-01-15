using QuanLyTaiSanTest.Models;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
       public Task<Inventory> Create(Inventory inventory);
       public Task<Inventory?> GetById(int id);
       public Task Update();
    }
}

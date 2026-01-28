using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Enum;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;

namespace QuanLyTaiSanTest.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Inventory> Create(Inventory inventory)
        {
            await _context.Inventory.AddAsync(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<(List<Inventory> Items, int TotalCount)> GetAll(int pageIndex, int pageSize, int? departmentId, int? status)
        {

            var result = _context.Inventory.Include(h => h.User).Include(h => h.Department).OrderByDescending(h => h.InventoryId).AsQueryable();

            if (departmentId != null)
            {
                result = result.Where(h => h.Department.Id == departmentId);
            }
            if (status != null)
            {
                result = result.Where(h => h.Status == (InventoryStatus)status);
            }
            var totalCount = await result.CountAsync();
            var items = await result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<Inventory?> GetById(int id)
        {
            return await _context.Inventory.Include(h => h.User).Include(h => h.Department).FirstOrDefaultAsync(h => h.InventoryId==id);
        }

        public async Task Update()
        {
             await _context.SaveChangesAsync();
        }
    }
}

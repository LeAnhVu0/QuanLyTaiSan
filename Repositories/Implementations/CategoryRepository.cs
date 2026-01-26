using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using System.Globalization;

namespace QuanLyTaiSanTest.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<bool> CheckAssetInCategory(int categoryId)
        {
           return await _context.Assets.AnyAsync(h => h.CategoryId == categoryId && !h.IsDelete);
        }

        public async Task Create(Category category)
        {
            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Category category)
        {
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Category>> GetAll()
        {
           return await _context.Category.Include(h => h.Assets.Where(a => a.IsDelete == false)).ToListAsync();
        }
        public async Task<(List<Category> Items, int TotalCount)> GetPageList(int pageIndex, int pageSize, string? search, int? status, string sortBy, bool desc)
        {
            var list = _context.Category.Include(h => h.Assets.Where(a => a.IsDelete == false)).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(h => h.CategoryName.Contains(search));
            }
            if (status != null)
            {
                list = list.Where(h => h.Status == (CategoryStatus)status);
            }
            switch (sortBy.ToLower())
            {
                case "name":
                    list = desc ? list.OrderByDescending(h => h.CategoryName) : list.OrderBy(h => h.CategoryName);
                    break;
                case "date":
                    list = desc ? list.OrderByDescending(h => h.CreatedTime) : list.OrderBy(h => h.CreatedTime);
                    break;
                default:
                    list = list.OrderBy(h => h.CategoryName);
                    break;
            }
            var totalCount = await list.CountAsync();
            var listPage = await list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (listPage,totalCount);
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Category.Include(C=>C.Assets.Where(a => a.IsDelete == false)).FirstOrDefaultAsync(c => c.CategoryId == id);
        }
        public async Task Update()
        {
            await _context.SaveChangesAsync();
        }
    }
}

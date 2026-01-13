using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;

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
           return await _context.Assets.AnyAsync(h => h.CategoryId == categoryId);
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
            return await _context.Category.Include(C => C.Assets).ToListAsync();

        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Category.Include(C=>C.Assets).FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task Update()
        {
            await _context.SaveChangesAsync();
        }
    }
}

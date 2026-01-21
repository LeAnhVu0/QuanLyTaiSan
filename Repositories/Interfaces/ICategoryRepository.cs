using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetById(int id);
        public Task<bool> CheckAssetInCategory(int categoryId);
        public Task<(List<Category> Items, int TotalCount)> GetAll(int pageIndex, int pageSize, string? search, int? status, string sortBy, bool desc);
        public Task Create(Category category);
        public Task Update();
        public Task Delete(Category category);
    }
}

using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IAssetRepository
    {
        public Task<List<Asset>> GetAll(string? search, int? categoryId , int? status, int pageIndex , int pageSize);
        public Task<List<Asset>> SortAssets(string sortBy, bool desc);
        public Task<Asset?> GetById(int id);
        public Task<Asset> Create(Asset asset);
        public Task Update();
        public Task Delete(Asset asset);
        public Task<Asset?> GetLatesAssetByCategory(int categoryId);
    }
}

using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IAssetHistoryRepository
    {
        public Task AddAssetHistory(AssetHistory assetHistory);
        public Task<List<AssetHistory>> GetAll();
    }
}

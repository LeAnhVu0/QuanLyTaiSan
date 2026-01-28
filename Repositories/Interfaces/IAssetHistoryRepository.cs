using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IAssetHistoryRepository
    {
        public Task AddAssetHistory(AssetHistory assetHistory);
        public Task<(List<AssetHistory> items, int totalCount)> GetAll(int pageIndex, int pageSize, int? assetId, string? searchName, string? actionType);
        public Task<List<AssetHistory>> GetById(int assetId);
    }
}

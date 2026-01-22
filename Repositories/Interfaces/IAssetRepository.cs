using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
using System.Linq.Expressions;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IAssetRepository
    {
        public Task<List<Asset>> GetAll();
        public Task<(List<Asset> Items, int TotalCount)> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, int? status, string sortBy, bool desc);
        public Task<Asset?> GetById(int id);
        public Task<Asset> Create(Asset asset);
        public Task Update();
        public Task Delete(Asset asset);
        public Task<bool> AnyAsync(Expression<Func<AssetTransfer, bool>> predicate);
        public Task<Asset?> GetLatesAssetByCategory(int categoryId);
        public Task AddTransfer(AssetTransfer assetTransfer);
        public Task<AssetTransfer?> GetTransferById(int transferId);
        public Task<(List<AssetTransfer> Items , int TotalCount)> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type);

    }
}

using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;

namespace QuanLyTaiSanTest.Repositories.Implementations
{
    public class AssetHistoryRepository : IAssetHistoryRepository
    {
        private readonly AppDbContext _context;

        public AssetHistoryRepository(AppDbContext context) 
        {
            _context = context;
        }
        public async Task AddAssetHistory(AssetHistory assetHistory)
        {
            await _context.AssetHistory.AddAsync(assetHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<AssetHistory> items, int totalCount)> GetAll(int pageIndex, int pageSize, int? assetId, string? searchName, string? actionType)
        {
            var list = _context.AssetHistory.Include(h=>h.CreatedByUser)
                                            .Include(h=>h.AssignedToUser)
                                            .OrderByDescending(h => h.ActionDate)
                                            .AsQueryable();
            if(assetId != null)
            {

                list = list.Where(h => h.Asset.AssetId == assetId);
            }    
            if(!string.IsNullOrEmpty(searchName))
            {
                list = list.Where(h => h.AssignedToUser.UserName == searchName || h.CreatedByUser.UserName == searchName || h.Descriptions.Contains(searchName));
            }    
            if(!string.IsNullOrEmpty(actionType))
            {
                list = list.Where(h => h.ActionType == actionType);
            }
            var totalCount = await list.CountAsync();
            var items = await list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
        public async Task<List<AssetHistory>> GetById(int assetId)
        {
            return await _context.AssetHistory.Include(h => h.CreatedByUser).Include(h => h.AssignedToUser)
                .Where(h => h.Asset.AssetId == assetId).ToListAsync();
        }
    }
}

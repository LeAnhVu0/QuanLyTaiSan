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

        public async Task<List<AssetHistory>> GetAll()
        {
            return await _context.AssetHistory.ToListAsync();
        }
    }
}

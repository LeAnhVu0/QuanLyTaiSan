using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;

namespace QuanLyTaiSanTest.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _dbcontext;

        public ReportRepository(AppDbContext dbcontext) 
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<Report>> GetAllReport()
        {
            return await _dbcontext.Report.ToListAsync();
        }
        public async Task AddReport(Report report)
        {
            await _dbcontext.AddAsync(report);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<List<Asset>> GetCategoryDetails()
        {
            return await _dbcontext.Assets.Include(a => a.Category)
                                    .OrderBy(a => a.Category.CategoryName)
                                    .ToListAsync();
        }
        
        public async Task<List<AssetStatistic>> GetCategoryAnalytics()
        {
            var list = from a in _dbcontext.Assets
                        where a.Category != null
                        group a by a.Category.CategoryName into g
                        select new AssetStatistic
                        {
                            GroupName = g.Key,
                            Quantity = g.Count(),
                            TotalValue = g.Sum(x => x.OriginalValue)
                        };
            return await list.ToListAsync();
        }
    }
}

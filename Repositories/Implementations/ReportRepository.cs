using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Report;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Enum;
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

        public async Task<List<AssetStatistic>> GetDepartmentAnalytics()
        {
            return await _dbcontext.Assets
            .Include(a => a.Department)
            .GroupBy(a => a.Department.DepartmentName)
            .Select(g => new AssetStatistic
            {
                GroupName = g.Key ?? "Chưa phân bổ",
                Quantity = g.Count(),
                TotalValue = g.Sum(x => x.OriginalValue)
            })
            .ToListAsync();
        }

        public async Task<List<AssetStatistic>> GetStatusAnalytics()
        {
            return await _dbcontext.Assets
            .GroupBy(a => a.Status)
            .Select(g => new AssetStatistic
            {
                GroupName = ((AssetStatus)g.Key).ToString(),
                Quantity = g.Count(),
                TotalValue = g.Sum(x => x.OriginalValue)
            })
            .ToListAsync();
        }

        // Báo cáo Tăng/Giảm theo thời gian
        public async Task<ReportFluctuationDto> GetFluctuationReport(DateTime fromDate, DateTime toDate)
        {
            // Tài sản tăng: Dựa vào ngày mua nằm trong khoảng
            var newAssets = await _dbcontext.Assets
                .Where(a => a.CreatedTime >= fromDate && a.CreatedTime <= toDate)
                .ToListAsync();

            // Tài sản giảm: Dựa vào Trạng thái là "Thanh lý" hoặc "Hỏng" 

            var liquidatedAssets = await _dbcontext.Assets
                .Where(a => (a.Status == AssetStatus.ThanhLy || a.Status == AssetStatus.Mat)
                         && a.UpdatedTime >= fromDate && a.UpdatedTime <= toDate)
                .ToListAsync();

            return new ReportFluctuationDto
            {
                NewAssetsCount = newAssets.Count,
                NewAssetsValue = newAssets.Sum(a => a.OriginalValue),
                LiquidatedAssetsCount = liquidatedAssets.Count,
                LiquidatedAssetsValue = liquidatedAssets.Sum(a => a.OriginalValue)
            };

        }
    }
}

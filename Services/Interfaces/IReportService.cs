using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IReportService
    {
        public Task<List<ReportHistoryDto>> GetAllReport();
        public Task<List<ReportAnalyticsDto>> GetCategoryAnalytics();
        public Task<List<AssetDto>> GetCategoryDetails(int? userId);

    }
}

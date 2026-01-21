using QuanLyTaiSan.Dtos.Report;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IReportService
    {
        public Task<List<ReportHistoryDto>> GetAllReport();
        public Task<List<ReportAnalyticsDto>> GetCategoryAnalytics();
        public Task<List<ReportAnalyticsDto>> GetDepartmentAnalytics();
        public Task<List<ReportAnalyticsDto>> GetStatusAnalytics();
        public Task<ReportFluctuationDto> GetFluctuation(DateTime fromDate, DateTime toDate);
        public Task<List<AssetRespondDto>> GetCategoryDetails(int? userId);

    }
}

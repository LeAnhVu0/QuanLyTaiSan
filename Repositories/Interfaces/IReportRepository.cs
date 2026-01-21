using QuanLyTaiSan.Dtos.Report;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IReportRepository
    {

        public Task<List<Report>> GetAllReport();
        public Task AddReport(Report report);
        public Task<List<Asset>> GetCategoryDetails();
        public Task<List<AssetStatistic>> GetCategoryAnalytics();
        public Task<List<AssetStatistic>> GetDepartmentAnalytics();
        public Task<List<AssetStatistic>> GetStatusAnalytics();

        public Task<ReportFluctuationDto> GetFluctuationReport(DateTime fromDate, DateTime toDate);

    }
}

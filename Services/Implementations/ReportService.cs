using QuanLyTaiSan.Dtos.Report;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Report;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo) 
        {
            _repo = repo;
        }
        public async Task<List<ReportHistoryDto>> GetAllReport()
        {
            var list = await _repo.GetAllReport();
            return list.Select(h => new ReportHistoryDto
            {
                ReportId= h.ReportId,
                Title = h.Title,
                Type = h.Type,
                FilterJson = h.FilterJson,
                CreateTime = h.CreateTime,
                UpdateTime = h.UpdateTime,
                IsDeleted = h.IsDeleted,
                UserId = h.UserId
            }).ToList();
        }

        public async Task<List<ReportAnalyticsDto>> GetCategoryAnalytics()
        {
            var list = await _repo.GetCategoryAnalytics();
            return list.Select(h => new ReportAnalyticsDto
            {
                GroupName = h.GroupName,
                Quantity = h.Quantity, 
                TotalValue = h.TotalValue
            }).ToList();
        }

        public async Task<List<AssetRespondDto>> GetCategoryDetails(int? userId)
        {
            var data = await _repo.GetCategoryDetails();
            return data.Select( h => new AssetRespondDto
            {
                AssetCode = h.AssetCode,
                AssetName = h.AssetName,
                AssetId = h.AssetId,

                Descriptions = h.Descriptions,
                ImageAsset = h.ImageAsset,
                ManufactureYear = h.ManufactureYear,

                OriginalValue = h.OriginalValue,
                PurchaseDate = h.PurchaseDate,
                Status = h.Status.ToDisplayName(),
                Note = h.Note,
                Unit = h.Unit,
                CreatedTime = h.CreatedTime,
                UpdatedTime = h.UpdatedTime,
                CategoryId = h.CategoryId
            }).ToList();
        }

        public async Task<List<ReportAnalyticsDto>> GetDepartmentAnalytics()
        {
            var list = await _repo.GetDepartmentAnalytics();
            return list.Select(h => new ReportAnalyticsDto
            {
                GroupName = h.GroupName,
                Quantity = h.Quantity,
                TotalValue = h.TotalValue
            }).ToList();
        }

        public async Task<List<ReportAnalyticsDto>> GetStatusAnalytics()
        {
            var list = await _repo.GetStatusAnalytics();
            return list.Select(h => new ReportAnalyticsDto
            {
                GroupName = h.GroupName,
                Quantity = h.Quantity,
                TotalValue = h.TotalValue
            }).ToList();
        }

        public async Task<ReportFluctuationDto> GetFluctuation(DateTime fromDate, DateTime toDate)
        {
            var result = await _repo.GetFluctuationReport(fromDate, toDate);
            return new ReportFluctuationDto
            {
                NewAssetsCount = result.NewAssetsCount,
                NewAssetsValue = result.NewAssetsValue,
                LiquidatedAssetsCount = result.LiquidatedAssetsCount,
                LiquidatedAssetsValue = result.LiquidatedAssetsValue
            };
        }

    }
}

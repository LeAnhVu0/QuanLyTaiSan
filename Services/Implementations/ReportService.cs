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
         // khi nào xuất pdf thì dùng
            //var history = new Report
            //{
            //    Title = $"Báo cáo tài sản theo Loại - Xuất lúc {DateTime.Now:dd/MM/yyyy}",
            //    Type = ReportType.AssetByCategory,
            //    CreateTime = DateTime.Now,
            //    UserId = userId,
            //    IsDeleted = false,
            //    FilterJson = null
            //};
            //await _repo.AddReport(history);
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

    }
}

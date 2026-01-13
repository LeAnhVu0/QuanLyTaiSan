using QuanLyTaiSanTest.Dtos.AssetHistory;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class AssetHistoryService:IAssetHistoryService
    {
        private readonly IAssetHistoryRepository _repo;

        public AssetHistoryService(IAssetHistoryRepository repo) 
        {
            _repo = repo;
        }

        public async Task<List<AssetHistoryDto>> GetAll()
        {
            var listHistory = await _repo.GetAll();
            return  listHistory.Select(h => new AssetHistoryDto
            {
                HistoryID = h.HistoryID,
                ActionType = h.ActionType,
                ActionDate = h.ActionDate,
                Descriptions = h.Descriptions,
                AssetId = h.AssetId,
                AssetName = h.AssetName,
                OriginalValue = h.OriginalValue,
                PurchaseDate = h.PurchaseDate,
                Status = h.Status,
                CreatedTime = h.CreatedTime,
                UpdatedTime = h.UpdatedTime,
                UserId = h.UserId,
            }).ToList();
        }
    }
}

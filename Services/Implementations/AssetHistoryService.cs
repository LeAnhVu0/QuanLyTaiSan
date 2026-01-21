using QuanLyTaiSan.Dtos.Auth;
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
            return listHistory.Select(h => new AssetHistoryDto
            {
                HistoryID = h.HistoryID,
                ActionType = h.ActionType,
                ActionDate = h.ActionDate,
                Descriptions = h.Descriptions,
                AssetId = h.AssetId,
                AssetName = h.AssetName,
                Status = h.Status,
                CreatedByUserId = new UserDto
                {
                    Id = h.CreatedByUserId,
                    Username = h.CreatedByUser.UserName
                    //Email = h.CreatedByUser.Email,
                    //PhoneNumber = h.CreatedByUser.PhoneNumber
                },
                AssignedToUserId = h.AssignedToUserId == null ? null : new UserDto
                { 
                    Id = h.AssignedToUserId,
                    Username = h.AssignedToUser.UserName
                    //Email = h.AssignedToUser.Email,
                    //PhoneNumber = h.AssignedToUser.PhoneNumber
                }
            }).ToList();
        }
    }
}

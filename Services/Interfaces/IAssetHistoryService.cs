using QuanLyTaiSan.Dtos.AssetHistory;
using QuanLyTaiSanTest.Dtos.AssetHistory;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetHistoryService
    {
        public Task<AssetHistoryAllDto> GetAll(int pageIndex, int pageSize, string? searchName, string? actionType);
        public Task<List<AssetHistoryDto>> GetById(int assetId);

    }
}

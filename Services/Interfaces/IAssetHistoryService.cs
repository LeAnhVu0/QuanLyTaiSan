using QuanLyTaiSanTest.Dtos.AssetHistory;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetHistoryService
    {
        public Task<List<AssetHistoryDto>> GetAll();
        public Task<List<AssetHistoryDto>> GetById(int assetId);

    }
}

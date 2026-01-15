using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSanTest.Dtos;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetService
    {
        //public Task Handover(int assetId, int userId);
        //public Task RecallAsset(int assetId);

        public Task<AssetHandoverDto> AssetHandover(int assetId, string userId);
        public  Task AssetRecall(int assetId);

        public Task<AssetAllDto> GetAll(int pageIndex, int pageSize, string? search, int? categoryId, int? status);
        public Task<List<AssetDto>> SortAssets(string sortBy, bool desc);
        public Task<AssetDto> GetById(int id);
        public Task<AssetDto> Create(CreateAssetDto createAssetDto);
        public Task Update(UpdateAssetDto updateAssetDto, int id);
        public Task Delete(int id);
    }
}

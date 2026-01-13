using QuanLyTaiSanTest.Dtos;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetService
    {
        //public Task Handover(int assetId, int userId);
        //public Task RecallAsset(int assetId);


        public Task<List<AssetDto>> GetAll(string? search, int? categoryId, int? status, int pageIndex, int pageSize);
        public Task<List<AssetDto>> SortAssets(string sortBy, bool desc);
        public Task<AssetDto> GetById(int id);
        public Task<AssetDto> Create(CreateAssetDto createAssetDto);
        public Task Update(UpdateAssetDto updateAssetDto, int id);
        public Task Delete(int id);
    }
}

using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.AssetTransfer;
using QuanLyTaiSanTest.Dtos;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetService
    {
        public  Task<AssetFormHandoverDto> CreateFormHandover(CreateFormTransferDto createFormTransferDto);

        public  Task<AssetFormHandoverDto> CreateFormRecall(CreateFormTransferDto createFormTransferDto);
        public  Task<ProcessTransferResultDto> ProcessApproval(int transferID,ProcessTransferDto processTransferDto);
        public  Task<AssetTransferAllDto> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type);
        public Task<AssetAllDto> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, int? status, string sortBy, bool desc);
        public Task<List<AssetRespondDto>> GetAll();

        public Task<AssetDto> GetById(int id);
        public Task<AssetRespondDto> Create(CreateAssetDto createAssetDto);
        public Task<AssetRespondDto> Update(UpdateAssetDto updateAssetDto, int id);
        public Task Delete(int id);
    }
}

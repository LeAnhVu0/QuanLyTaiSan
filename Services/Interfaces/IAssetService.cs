using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.AssetTransfer;
using QuanLyTaiSanTest.Dtos;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface IAssetService
    {
        public Task<List<AssetFormHandoverDto>> CreateMultiFormHandover(CreateMultiFormTransferDto createMultiFormHandoverDto);
        public Task<List<AssetFormHandoverDto>> CreateMultiFormRecall(CreateMultiFormTransferDto createMultiFormRecallDto);
        public Task<List<AssetFormHandoverDto>> CreateMultiFormDepartmentMove(CreateMultiDepartmentFormTransferDto createMultiDepartmentFormTransferDto);

        public Task<List<ProcessTransferResultDto>> ProcessMultiApproval(ProcessMultiTransferDto processMultiTransferDto);



        public Task<AssetTransferAllDto> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type);
        public Task<AssetAllDto> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, string? userId, int? departmentId, int? status, string sortBy, bool desc);
        public Task<List<AssetRespondDto>> GetAll();

        public Task<AssetDto> GetById(int id);
        public Task<AssetDto> GetByCode(string code);
        public Task<AssetRespondDto> Create(CreateAssetDto createAssetDto);
        public Task<AssetRespondDto> Update(UpdateAssetDto updateAssetDto, int id);
        public Task Delete(int id);
    }
}

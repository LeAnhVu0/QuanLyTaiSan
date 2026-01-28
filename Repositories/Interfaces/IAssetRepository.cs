using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Models;
using System;
using System.Linq.Expressions;

namespace QuanLyTaiSanTest.Repositories.Interfaces
{
    public interface IAssetRepository
    {
       // public Task<List<Asset>> GetAll();
       // public Task<(List<Asset> Items, int TotalCount)> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, string? userId, int? departmentId, int? status, string sortBy, bool desc);
       // public Task<Asset?> GetById(int id);
       // public Task<Asset?> GetByCode(string code);
       // public Task<Asset> Create(Asset asset);
       // public Task Update();
       // public Task Delete(Asset asset);
       //// Kiểm tra xem có  đối tượng nào thỏa mãn điều kiện predicate hay không.
       // public Task<bool> AnyAsync(Expression<Func<AssetTransfer, bool>> predicate);
       // public Task<bool> AnyAssetAsync(Expression<Func<Asset, bool>> predicate);

       // public Task<List<Asset>> GetAssetsByIds(List<int> assetIds);
       // public Task<List<AssetTransfer>> GetTransfersByIds(List<int> transferIds);

       // //Hàm đếm số lượng tài sản trong phòng ban
       // public Task<int> CountAssetsByDepartment(int departmentId);

       // public Task<Asset?> GetLatesAssetByCategory(int categoryId);


       // public Task AddTransfer(AssetTransfer assetTransfer);
       // public Task<AssetTransfer?> GetTransferById(int transferId);
       // public Task<List<AssetTransfer>> GetTransfersByBatchId(Guid batchId);
       // public Task<(List<AssetTransfer> Items , int TotalCount)> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type);



        // Lấy tất cả tài sản (không phân trang)
        public Task<List<Asset>> GetAll();

        // Lấy danh sách tài sản có phân trang, tìm kiếm, lọc theo nhiều tiêu chí (loại, người giữ, phòng ban, trạng thái) và sắp xếp
        public Task<(List<Asset> Items, int TotalCount)> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, string? userId, int? departmentId, int? status, string sortBy, bool desc);

        // Lấy chi tiết một tài sản theo ID
        public Task<Asset?> GetById(int id);

        // Lấy chi tiết một tài sản theo Mã tài sản (AssetCode)
        public Task<Asset?> GetByCode(string code);

        // Thêm mới một tài sản vào Database
        public Task<Asset> Create(Asset asset);

        // Lưu các thay đổi xuống Database 
        public Task Update();

        // Xóa một tài sản khỏi Database
        public Task Delete(Asset asset);

        // Kiểm tra xem có phiếu (AssetTransfer) nào thỏa mãn điều kiện hay không 
        public Task<bool> AnyAsync(Expression<Func<AssetTransfer, bool>> predicate);

        // Kiểm tra xem có Tài sản (Asset) nào thỏa mãn điều kiện hay không
        public Task<bool> AnyAssetAsync(Expression<Func<Asset, bool>> predicate);

        // Lấy danh sách nhiều tài sản cùng lúc dựa trên danh sách ID 
        public Task<List<Asset>> GetAssetsByIds(List<int> assetIds);

        // Lấy danh sách nhiều phiếu cùng lúc dựa trên danh sách ID
        public Task<List<AssetTransfer>> GetTransfersByIds(List<int> transferIds);

        // Hàm đếm số lượng tài sản đang thuộc về một phòng ban 
        public Task<int> CountAssetsByDepartment(int departmentId);

        // Lấy tài sản mới nhất vừa được tạo của một Loại tài sản
        public Task<Asset?> GetLatesAssetByCategory(int categoryId);

        // Tạo mới một phiếu (Bàn giao, Thu hồi, Điều chuyển)
        public Task AddTransfer(AssetTransfer assetTransfer);

        // Lấy chi tiết một phiếu theo ID phiếu
        public Task<AssetTransfer?> GetTransferById(int transferId);

        // Lấy danh sách tất cả các phiếu thuộc cùng một Lô (Batch) để duyệt gộp
        public Task<List<AssetTransfer>> GetTransfersByBatchId(Guid batchId);

        // Lấy danh sách phiếu có phân trang, lọc theo trạng thái (Status) và loại phiếu (Type)
        public Task<(List<AssetTransfer> Items, int TotalCount)> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type);
    }
}

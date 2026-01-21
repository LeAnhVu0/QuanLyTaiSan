using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuanLyTaiSanTest.Repositories.Implementations
{
    public class AssetRepository:IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Asset> Create(Asset asset)
        {
            await _context.AddAsync(asset);
            await _context.SaveChangesAsync();
            return asset;
        }
        public async Task Update()
        {
             await _context.SaveChangesAsync();
        }
        public async Task Delete(Asset asset)
        {
            _context.Assets.Remove(asset);
           await _context.SaveChangesAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<AssetTransfer, bool>> predicate)
        {
            return await _context.AssetTransfer.AnyAsync(predicate);
        }
        public async Task<(List<Asset> Items, int TotalCount)> GetAll(int pageIndex, int pageSize, string? search, int? categoryId, int? status, string sortBy, bool desc)
        {
            var listAsset =  _context.Assets.Where(h => h.IsDelete == false).Include(h => h.Category).AsQueryable();
            if(!string.IsNullOrEmpty(search))
            {
                listAsset = listAsset.Where(h=>h.AssetName.Contains(search));
            }
            if (categoryId != null && categoryId>0)
            {
                listAsset = listAsset.Where(h => h.CategoryId == categoryId);
            }
            if (status!= null)
            {
                listAsset = listAsset.Where(h => h.Status == (AssetStatus)status);
            }

            if (string.IsNullOrWhiteSpace(sortBy))
                sortBy = "name";
            switch (sortBy.ToLower())
            {
                case "name":
                    listAsset = desc ? listAsset.OrderByDescending(h => h.AssetName) : listAsset.OrderBy(h => h.AssetName);
                    break;

                case "date":
                    listAsset = desc ? listAsset.OrderByDescending(h => h.CreatedTime) : listAsset.OrderBy(h => h.CreatedTime);
                    break;

                case "price":
                    listAsset = desc ? listAsset.OrderByDescending(h => h.OriginalValue) : listAsset.OrderBy(h => h.OriginalValue);
                    break;

                default:
                    listAsset = listAsset.OrderBy(a => a.AssetName);
                    break;
            }
            var totalCount = await listAsset.CountAsync();

            //page
            var list = await listAsset.Skip((pageIndex-1)*pageSize).Take(pageSize).ToListAsync();
            return (list, totalCount);
        }

        public async Task<Asset?> GetById(int id)
        {
            return await _context.Assets.Where(h => h.IsDelete == false)
                                        .Include(h=>h.Category)
                                        .Include(h => h.Department)
                                        .Include(h => h.User)
                                        .FirstOrDefaultAsync(h=>h.AssetId==id);
        }
        public async Task<Asset?> GetLatesAssetByCategory(int categoryId)
        {
            return await _context.Assets.Where(h => h.CategoryId == categoryId && h.IsDelete == false)
                                        .OrderByDescending(h => h.AssetId)
                                        .FirstOrDefaultAsync();
        }

        public async Task AddTransfer(AssetTransfer assetTransfer)
        {
           _context.AssetTransfer.Add(assetTransfer);
            await _context.SaveChangesAsync();
        }

        public async Task<AssetTransfer?> GetTransferById(int transferId)
        {
            return await _context.AssetTransfer.Include(h => h.Asset).FirstOrDefaultAsync(h => h.TransferId == transferId);
        }

        public async Task<List<AssetTransfer>> GetAllTransfer(int pageIndex, int pageSize, int? status , int? type)
        {
            var list = _context.AssetTransfer
        .Include(t => t.Asset)
        .Include(t => t.Department)      // Join bảng Phòng ban
        .Include(t => t.FromUser)        // Join người gửi
        .Include(t => t.ToUser)          // Join người nhận
        .Include(t => t.CreatedByUser)   // Join người tạo
        .Include(t => t.ApprovedByUser)  // Join người duyệt
        .AsQueryable();
            if (status != null)
            {
                list = list.Where(h => h.Status == (AssetTransferStatus)status);
            }
            if (type != null)
            {
                list = list.Where(h => h.TransferType == (AssetTransferType)type);
            }
            var listResult = await list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return listResult;
        }
    }
}

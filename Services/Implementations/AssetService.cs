using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Services.Implementations;
using QuanLyTaiSan.Services.Interfaces;
using QuanLyTaiSanTest.Dtos.Asset;

using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repo;
        private readonly ICategoryRepository _repoCate;
        private readonly IAssetHistoryRepository _repoHistory;
        private readonly IAuthService _authService;
        private readonly IDepartmentService _departmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssetService(IAssetRepository repo, ICategoryRepository repoCate, IAssetHistoryRepository repoHistory,
            IAuthService authService, IDepartmentService departmentService , IHttpContextAccessor httpContextAccessor )
        {
            _repo = repo;
            _repoCate = repoCate;
            _repoHistory = repoHistory;
            _authService = authService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AssetHandoverDto> AssetHandover(int assetId , string userId)
        {
            var asset = await _repo.GetById(assetId);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Không tồn tại tài sản có id = {assetId}");
            }
            var user = await  _authService.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Không tồn tại tài khoản có id = {userId}");
            }
            if (asset.DepartmentId != user.DepartmentId)
            {
                throw new InvalidOperationException("Tài sản này không thuộc phòng ban của người nhận");
            }
            if (asset.Status != AssetStatus.SanSang)
            {
                throw new InvalidOperationException("Tài sản không ở trạng thái sẵn sàng để bàn giao");
            }
            if (!string.IsNullOrEmpty(asset.UserId))
            {
                throw new InvalidOperationException("Tài sản đã được cấp phát cho người khác");
            }

            asset.UserId = userId;
            asset.Status = AssetStatus.DangSuDung;
            asset.UpdatedTime = DateTime.UtcNow;
            await _repo.Update();

            return new AssetHandoverDto
            {
                AssetId = assetId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                AssignedToUserId = userId,
                AssignedToUserName = user.Username,
                Status =asset.Status.ToDisplayName(),
                UpdatedAt = asset.UpdatedTime
            };
        }
        public async Task AssetRecall(int assetId)
        {
            var asset = await _repo.GetById(assetId);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Không tồn tại tài sản có id = {assetId}");
            }
            if (string.IsNullOrEmpty(asset.UserId))
                throw new InvalidOperationException("Tài sản này không được cấp cho ai nên không thể thu hồi");
            // Reset trạng thái
            asset.UserId = null;
            asset.Status = AssetStatus.SanSang;
            asset.UpdatedTime = DateTime.UtcNow;
            asset.DepartmentId = 3;
            await _repo.Update();
        }

        public async Task<AssetDto> Create(CreateAssetDto createAssetDto)
        {
            if (createAssetDto == null)
            {
                throw new BadHttpRequestException("Dữ liệu gửi lên bị null");
            }
            var category = await _repoCate.GetById(createAssetDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Mã loại tài sản không tồn tại");
            }
           
            if ((await _departmentService.GetDepartmentById(createAssetDto.DepartmentId)) == null)
            {
                throw new KeyNotFoundException("Không tồn tại phòng ban có id = " + createAssetDto.DepartmentId);
            }
            var words = category.CategoryName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string prefix = "";
            foreach (var word in words)
            {
                prefix += word[0];
            }
            prefix = prefix.ToUpper();
            var lastAsset = await _repo.GetLatesAssetByCategory(createAssetDto.CategoryId);

            int nextNumber = 1;
            if (lastAsset != null)
            {
                string number = lastAsset.AssetCode.Replace(prefix, "");
                nextNumber = int.Parse(number) + 1;
            }
           
            var h = new Asset
            {
                AssetCode = prefix + nextNumber,
                AssetName = createAssetDto.AssetName,
                Descriptions = createAssetDto.Descriptions,
                ImageAsset = createAssetDto.ImageAsset,
                ManufactureYear = createAssetDto.ManufactureYear,
                OriginalValue = createAssetDto.OriginalValue,
                PurchaseDate = createAssetDto.PurchaseDate,
                Unit = createAssetDto.Unit,
                Status = AssetStatus.SanSang,
                Note = createAssetDto.Note,
                CreatedTime = DateTime.Now,
                CategoryId = createAssetDto.CategoryId,
                DepartmentId = createAssetDto.DepartmentId
            };
            // LẤY USER ID RA
            //string currentUserId = GetCurrentUserId(); 
            await _repo.Create(h);
            //await SaveHistory(h, "CREATE", $"Thêm mới tài sản từ loại ID: {h.CategoryId}",currentUserId);
            return new AssetDto
            {
                AssetCode = h.AssetCode,
                AssetName = h.AssetName,
                AssetId = h.AssetId,

                Descriptions = h.Descriptions,
                ImageAsset = h.ImageAsset,
                ManufactureYear = h.ManufactureYear,

                OriginalValue = h.OriginalValue,
                PurchaseDate = h.PurchaseDate,
                Status = h.Status.ToDisplayName(),
                Note = h.Note,
                Unit = h.Unit,
                CreatedTime = h.CreatedTime,
                UpdatedTime = h.UpdatedTime,
                CategoryId = h.CategoryId,
                DepartmentId = h.DepartmentId
            };
        }
        public async Task Update(UpdateAssetDto updateAssetDto, int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có id = " + id);
            }
            if ((await _repoCate.GetById(updateAssetDto.CategoryId)) == null)
            {
                throw new KeyNotFoundException("Không tồn tại loại có id = " + updateAssetDto.CategoryId);
            }
            if ((await _departmentService.GetDepartmentById(updateAssetDto.DepartmentId)) == null)
            {
                throw new KeyNotFoundException("Không tồn tại phòng ban có id = " + updateAssetDto.DepartmentId);
            }
            else
            {
                h.AssetName = updateAssetDto.AssetName;
                h.Descriptions = updateAssetDto.Descriptions;
                h.ImageAsset = updateAssetDto.ImageAsset;
                h.ManufactureYear = updateAssetDto.ManufactureYear;
                h.OriginalValue = updateAssetDto.OriginalValue;
                h.PurchaseDate = updateAssetDto.PurchaseDate;
                h.Status = (AssetStatus)updateAssetDto.Status;
                h.Note = updateAssetDto.Note;
                h.Unit = updateAssetDto.Unit;
                h.CategoryId = updateAssetDto.CategoryId;
                h.UpdatedTime = DateTime.Now;
                h.DepartmentId = updateAssetDto.DepartmentId;
                await _repo.Update();
                //string currentUserId = GetCurrentUserId();
                //await SaveHistory(h, "UPDATE", "Người dùng cập nhật thông tin tài sản ID : " + id,currentUserId);
            }
        }
    
        public async Task Delete(int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có id = " + id);
            }
            else
            {
                //await SaveHistory(h, "DELETE", "Xóa tài sản khỏi hệ thống",GetCurrentUserId());
                await _repo.Delete(h);
            }
        }

        public async Task<AssetAllDto> GetAll(int pageIndex, int pageSize,string? search, int? categoryId, int? status)
        {
            var data = await _repo.GetAll(pageIndex, pageSize, search, categoryId, status);
            if (data.Items == null || data.Items.Count == 0)
            {
                return null;
            }

            var items = data.Items.Select(h => new AssetDto
            {
                AssetCode = h.AssetCode,
                AssetName = h.AssetName,
                AssetId = h.AssetId,

                Descriptions = h.Descriptions,
                ImageAsset = h.ImageAsset,
                ManufactureYear = h.ManufactureYear,

                OriginalValue = h.OriginalValue,
                PurchaseDate = h.PurchaseDate,
                Status = h.Status.ToDisplayName(),
                Note = h.Note,
                Unit = h.Unit,
                CreatedTime = h.CreatedTime,
                UpdatedTime = h.UpdatedTime,
                CategoryId = h.CategoryId,
                DepartmentId = h.DepartmentId,
                UserId = h.UserId
            }).ToList();

            var totalPage = (int)Math.Ceiling(data.TotalCount / (double)pageSize);

            return new AssetAllDto
            {
                ListAsset = items,
                categoryId = categoryId,
                SearchName = search,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = data.TotalCount,
                TotalPage = totalPage,

                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPage
            };

        }
        public async Task<List<AssetDto>> SortAssets(string sortBy, bool desc)
        {
            var assets = await _repo.SortAssets(sortBy, desc);
            if (assets == null)
            {
                throw new KeyNotFoundException("Không có dữ liệu tài sản nào.");
            }
            else
            {
                return assets.Select(h => new AssetDto
                {
                    AssetCode = h.AssetCode,
                    AssetName = h.AssetName,
                    AssetId = h.AssetId,

                    Descriptions = h.Descriptions,
                    ImageAsset = h.ImageAsset,
                    ManufactureYear = h.ManufactureYear,

                    OriginalValue = h.OriginalValue,
                    PurchaseDate = h.PurchaseDate,
                    Status = h.Status.ToDisplayName(),
                    Note = h.Note,
                    Unit = h.Unit,
                    CreatedTime = h.CreatedTime,
                    UpdatedTime = h.UpdatedTime,
                    CategoryId = h.CategoryId
                }).ToList();
            }
        }

        public async Task<AssetDto> GetById(int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có id = " + id);
            }
            else
            {
                return new AssetDto
                {
                    AssetCode = h.AssetCode,
                    AssetName = h.AssetName,
                    AssetId = h.AssetId,

                    Descriptions = h.Descriptions,
                    ImageAsset = h.ImageAsset,
                    ManufactureYear = h.ManufactureYear,

                    OriginalValue = h.OriginalValue,
                    PurchaseDate = h.PurchaseDate,
                    Status = h.Status.ToDisplayName(),
                    Note = h.Note,
                    Unit = h.Unit,
                    CreatedTime = h.CreatedTime,
                    UpdatedTime = h.UpdatedTime,
                    CategoryId = h.CategoryId
                };
            }
        }
        //Hàm xư lý lưu lịch sử
        private async Task SaveHistory(Asset asset, string actionType, string note, string? CreatedByUserId = null, string AssignedToUserId=null)
        {
            try
            {
                if (string.IsNullOrEmpty(CreatedByUserId))
                {
                    throw new Exception("Lỗi: CreatedByUserId bị Null. Bạn đã gửi Token đăng nhập chưa?");
                }

                var history = new AssetHistory
                {
                    AssetId = asset.AssetId,
                    ActionType = actionType,
                    Descriptions = note,
                    ActionDate = DateTime.Now,
                    AssetName = asset.AssetName,
                    Status = (int)asset.Status,

                    CreatedByUserId = CreatedByUserId,
                    AssignedToUserId = AssignedToUserId
                };

                await _repoHistory.AddAssetHistory(history);
            }
            catch (DbUpdateException ex) 
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Lỗi Database: {innerMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi Logic: {ex.Message}");
            }
        }
        //Hàm lấy thông tin người dùng 
        private string? GetCurrentUserId()
        {
            // Lấy User name từ Token 
            var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            return username ?? string.Empty;
        }

    }
}

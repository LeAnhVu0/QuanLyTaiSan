using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.NewFolder;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Runtime.InteropServices;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repo;
        private readonly ICategoryRepository _repoCate;
        private readonly IAssetHistoryRepository _repoHistory;
        private readonly IHttpClientFactory _httpClientFactory;

        public AssetService(IAssetRepository repo, ICategoryRepository repoCate, IAssetHistoryRepository repoHistory , IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _repoCate = repoCate;
            _repoHistory = repoHistory;
            _httpClientFactory = httpClientFactory;
        }
        ////Bàn giao tài sản
        //public async Task Handover(int assetId , int userId)
        //{
        //    //Kiểm tra tài sản có tồn tại hay không
        //    var asset = await _repo.GetById(assetId);
        //    if (asset == null)
        //    {
        //        throw new KeyNotFoundException("Không tồn tại tài sản có id = " + assetId);
        //    }
        //    try
        //    {
        //        // gọi api để lấy thông tin user
        //        var client = _httpClientFactory.CreateClient();
        //        var response = await client.GetAsync("https://8m52k23j-7235.asse.devtunnels.ms/api/Auth/Login");

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            throw new KeyNotFoundException("Không tìm thấy người dùng bên hệ thống User");
        //        }
        //        // chuyển json sang dữ liệu trong object
        //        var user = await response.Content.ReadFromJsonAsync<ExternalUserDto>();
        //        //check lỗi
        //        if (user == null)
        //        {
        //            throw new ExternalException("Lỗi dữ liệu trả về từ API User");
        //        }
        //        if (asset.DepartmentId != user.DepartmentId)
        //        {
        //            throw new InvalidOperationException("Không thể bàn giao: Nhân viên không thuộc phòng ban quản lý tài sản này");
        //        }
        //        if (asset.UserId != null)
        //        {
        //            throw new InvalidOperationException("Tài sản đã được sử dụng bởi người khác");
        //        }
        //        asset.UserId = userId;
        //        asset.Status = AssetStatus.DangSuDung;
        //        asset.UpdatedTime = DateTime.Now;
        //        await _repo.Update();
        //        await SaveHistory(asset, "HANDOVER", $"Bàn giao tài sản {assetId} cho user {userId}", userId);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new HttpRequestException("Không thể kết nối đến dịch vụ User.");
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        throw new TaskCanceledException("Dịch vụ User phản hồi quá chậm.");
        //    }
        //}
        ////Thu hồi tài sản
        //public async Task RecallAsset(int assetId)
        //{
        //    var asset = await _repo.GetById(assetId);
        //    if(asset == null)
        //    {
        //        throw new KeyNotFoundException($"Không tồn tại tài sản có id = {assetId}");
        //    }
        //    asset.UserId = null;
        //    asset.DepartmentId = 0;
        //    asset.Status = AssetStatus.SanSang;
        //    asset.UpdatedTime = DateTime.Now;
        //    await _repo.Update();
        //    await SaveHistory(asset, "RECALL", $"Thu hồi tài sản có id {assetId}");
        //}

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
            ////lấy thông tin phòng ban
            //var client = _httpClientFactory.CreateClient();
            //var response = await client.GetAsync($"https://8m52k23j-7235.asse.devtunnels.ms/api/Department/{createAssetDto.DepartmentId}");
            //if(!response.IsSuccessStatusCode)
            //{
            //    throw new KeyNotFoundException("Phòng ban không tồn tại");
            //}    

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

            await _repo.Create(h);
            await SaveHistory(h, "CREATE", $"Thêm mới tài sản từ loại ID: {h.CategoryId}");
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
                await SaveHistory(h, "UPDATE", "Người dùng cập nhật thông tin tài sản ID : " + id);
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
                await SaveHistory(h, "DELETE", "Xóa tài sản khỏi hệ thống");
                await _repo.Delete(h);
            }
        }

        public async Task<List<AssetDto>> GetAll(string? search, int? categoryId, int? status, int pageIndex, int pageSize)
        {
            var assets = await _repo.GetAll(search,categoryId,status,pageIndex,pageSize);
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
                    CategoryId = h.CategoryId,
                    DepartmentId = h.DepartmentId,
                    UserId = h.UserId

                }).ToList();
            }
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
        private async Task SaveHistory(Asset asset, string actionType, string note, int? userId = null)
        {
            var history = new AssetHistory
            {
                AssetId = asset.AssetId,
                ActionType = actionType,
                Descriptions = note,
                ActionDate = DateTime.Now,
                AssetName = asset.AssetName,
                OriginalValue = asset.OriginalValue,
                Status = (int)asset.Status,
                PurchaseDate = asset.PurchaseDate,
                CreatedTime = asset.CreatedTime,
                UpdatedTime = asset.UpdatedTime,
                UserId = userId
            };
            
            await _repoHistory.AddAssetHistory(history);
        }

    }
}

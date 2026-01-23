using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.AssetTransfer;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Implementations;
using QuanLyTaiSan.Services.Interfaces;
using QuanLyTaiSanTest.Dtos.Asset;

using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly UserManager<ApplicationUser> _userManager;

        public AssetService(IAssetRepository repo, ICategoryRepository repoCate, IAssetHistoryRepository repoHistory,
            IAuthService authService, IDepartmentService departmentService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _repoCate = repoCate;
            _repoHistory = repoHistory;
            _authService = authService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<AssetFormHandoverDto> CreateFormHandover(CreateFormTransferDto createFormTransferDto)
        {
            //Kiểm tra đầu vào null không
            if (createFormTransferDto == null)
                throw new ArgumentNullException(nameof(createFormTransferDto));

            if (string.IsNullOrEmpty(createFormTransferDto.ToUserId))
                throw new ArgumentException("Vui lòng chọn nhân viên nhận bàn giao");

            if (createFormTransferDto.DepartmentId == null)
                throw new ArgumentException("Vui lòng chọn phòng ban nhận bàn giao");

            //Kiêm tra tài sản có tồn tại hay không
            var asset = await _repo.GetById(createFormTransferDto.AssetId);
            if (asset == null) throw new KeyNotFoundException("Tài sản không tồn tại");

            //Check xung đột phiếu
            bool isPending = await _repo.AnyAsync(x =>x.AssetId == asset.AssetId &&x.Status == AssetTransferStatus.Pending);
            if (isPending)
            {
                throw new InvalidOperationException("Tài sản này đang có phiếu bàn giao/thu hồi chờ duyệt");
            }

            //Check xem tài sản đã được bàn giao hay chưa

            if (!string.IsNullOrEmpty(asset.UserId))
            {
                throw new InvalidOperationException("Tài sản đang được giữ bởi người khác. Phải thu hồi trước");
            }

            //Check trạng thái tài sản
            if (asset.Status != AssetStatus.SanSang)
            {
                throw new InvalidOperationException("Tài sản không ở trạng thái sẵn sàng để bàn giao");
            }

            //Kiểm tra người dùng có tồn tại hay không
            var user = await _authService.GetUserById(createFormTransferDto.ToUserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Không tồn tại tài khoản có id = {createFormTransferDto.ToUserId}");
            }

            //Kiểm tra phòng ban có tồn tại hay không
            var department = await _departmentService.GetDepartmentById(createFormTransferDto.DepartmentId.Value);

            if (department == null)
                throw new KeyNotFoundException("Phòng ban không tồn tại");


            //check xem tài sản , nhân viên có cùng thuộc 1 phòng ban không
            if (asset.DepartmentId != createFormTransferDto.DepartmentId)
            {
                throw new InvalidOperationException($"Tài sản ID = {asset.AssetId} không thuộc về phòng ban ID = {createFormTransferDto.DepartmentId}");
            }
            if (user.DepartmentId != createFormTransferDto.DepartmentId)
            {
                throw new InvalidOperationException($"Nhân viên không thuộc về phòng ban ID = {createFormTransferDto.DepartmentId}");
            }
            if (asset.DepartmentId != user.DepartmentId)
            {
                throw new InvalidOperationException("Tài sản và nhân viên không cùng thuộc 1 phòng ban");
            }

            var transfer = new AssetTransfer
            {
                AssetId = asset.AssetId,
                TransferType = QuanLyTaiSan.Enum.AssetTransferType.Handover,
                Status = QuanLyTaiSan.Enum.AssetTransferStatus.Pending,
                DepartmentId = createFormTransferDto.DepartmentId,
                FromUserId = null,
                ToUserId = createFormTransferDto.ToUserId,
                CreatedByUserId = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                Purpose = createFormTransferDto.Purpose,
                Note = createFormTransferDto.Note
            };
            await _repo.AddTransfer(transfer);
            return new AssetFormHandoverDto
            {
                TransferId = transfer.TransferId,
                AssetId = transfer.AssetId,
                TransferType = transfer.TransferType.ToDisplayName(),
                Status = transfer.Status.ToDisplayName(),
                DepartmentId = transfer.DepartmentId,
                FromUserId = transfer.FromUserId,
                ToUserId = transfer.ToUserId,
                CreatedByUserId = transfer.CreatedByUserId,
                Purpose = transfer.Purpose,
                Note = transfer.Note,
                CreatedAt = transfer.CreatedAt
            };
        }
        public async Task<AssetFormHandoverDto> CreateFormRecall(CreateFormTransferDto createFormTransferDto)
        {
            // kiểm tra xem nhân viên và tài sản có tồn tại không
            var asset = await _repo.GetById(createFormTransferDto.AssetId);
            if (asset == null) throw new KeyNotFoundException("Tài sản không tồn tại");

            var user = await _authService.GetUserById(createFormTransferDto.ToUserId);
            if (user == null) throw new KeyNotFoundException("Nhân viên không tồn tại");

            //Check xung đột phiếu
            bool isPending = await _repo.AnyAsync(x =>x.AssetId == asset.AssetId &&x.Status == AssetTransferStatus.Pending);
            if (isPending)
            {
                throw new InvalidOperationException("Tài sản này đang có phiếu chờ duyệt");
            }

            // kiểm tra xem tài sản có đang được sử dụng không 
            if (string.IsNullOrEmpty(asset.UserId))
                throw new InvalidOperationException("Tài sản chưa được cấp phát, không thể thu hồi");

            // kiểm tra xem nhân viên có sở hữu  tài sản này không 
            if (asset.UserId != createFormTransferDto.ToUserId)
                throw new InvalidOperationException("Nhân viên không sở hữu tài sản này");

            var transfer = new AssetTransfer
            {
                AssetId = createFormTransferDto.AssetId,
                TransferType = QuanLyTaiSan.Enum.AssetTransferType.Recall,
                Status = QuanLyTaiSan.Enum.AssetTransferStatus.Pending,
                DepartmentId = createFormTransferDto.DepartmentId,
                FromUserId = asset.UserId,
                ToUserId = null,
                CreatedByUserId = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                Purpose = createFormTransferDto.Purpose,
                Note = createFormTransferDto.Note
            };
            await _repo.AddTransfer(transfer);
            return new AssetFormHandoverDto
            {
                TransferId = transfer.TransferId,
                AssetId = transfer.AssetId,
                TransferType = transfer.TransferType.ToDisplayName(),
                Status = transfer.Status.ToDisplayName(),
                DepartmentId = transfer.DepartmentId,

                FromUserId = transfer.FromUserId,
                ToUserId = transfer.ToUserId,
                CreatedByUserId = transfer.CreatedByUserId,
                Note = transfer.Note,
                Purpose = transfer.Purpose,
                CreatedAt = transfer.CreatedAt
            };

        }
        
        //Quy trình xác nhận
        public async Task<ProcessTransferResultDto> ProcessApproval(int transferID, ProcessTransferDto processTransferDto)
        {
            // Kiểm tra xem phiếu có tồn tại không và đang trong trạng thái chờ duyệt hay không
            var transfer = await _repo.GetTransferById(transferID);
            if (transfer == null)
            {
                throw new KeyNotFoundException("Không tồn tại phiếu");
            }
            if (transfer.Status != AssetTransferStatus.Pending)
            {
                throw new InvalidOperationException("Phiếu không ở trong trạng thái chờ duyệt");
            }
             //kiểm tra xem tài sản có dữ liệu không
            var asset = transfer.Asset;
            if (asset == null)
                throw new KeyNotFoundException("Dữ liệu tài sản bị trống");

            //Kiểm tra người duyệt có tồn tại không
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                throw new UnauthorizedAccessException("Không xác định được người duyệt");

            // Xử lý từ chối duyệt phiếu
            if (processTransferDto.IsApproved == false)
            {
                transfer.Status = AssetTransferStatus.Rejected;
                transfer.ApprovedByUserId = currentUserId;
                transfer.ApprovedAt = DateTime.Now;
                transfer.RejectReason = processTransferDto.RejectReason ?? "Không có lý do";

                await _repo.Update();
                await SaveHistory(asset, "TRANSFER_REJECTED", $"Từ chối phiếu {transfer.TransferId}: {processTransferDto.RejectReason}", currentUserId, asset.UserId);
                return BuildResultDto(transfer);
            }
            //Xử lý duyệt phiếu
            transfer.Status = AssetTransferStatus.Approved;
            transfer.ApprovedByUserId = currentUserId;
            transfer.ApprovedAt = DateTime.Now;

            string actionType;
            string note;
            string? assignedUserId = null;
            if (transfer.TransferType == AssetTransferType.Handover)
            {
                transfer.Asset.UserId = transfer.ToUserId;
                transfer.Asset.Status = AssetStatus.DangSuDung;

                assignedUserId = transfer.ToUserId;
                actionType = "HANDOVER";
                note = $"Bàn giao tài sản cho nhân viên ID: {transfer.ToUserId}";
            }
            else
            {
                transfer.Asset.UserId = null;
                transfer.Asset.Status = AssetStatus.SanSang;

                actionType = "RECALL";
                note = $"Thu hồi tài sản từ nhân viên ID :{transfer.FromUserId}";
            }
            transfer.Asset.UpdatedTime = DateTime.UtcNow;
            transfer.Status = AssetTransferStatus.Completed;

            await _repo.Update();
            await SaveHistory(asset, actionType, note, currentUserId, assignedUserId);

            return BuildResultDto(transfer);
        }

        private ProcessTransferResultDto BuildResultDto(AssetTransfer transfer)
        {
            return new ProcessTransferResultDto
            {
                TransferId = transfer.TransferId,
                TransferType = transfer.TransferType.ToDisplayName(),
                Status = transfer.Status.ToDisplayName(),


                AssetId = transfer.Asset.AssetId,
                AssetCode = transfer.Asset.AssetCode,
                AssetName = transfer.Asset.AssetName,

                DepartmentId = transfer.DepartmentId,
                FromUserId = transfer.FromUserId,
                ToUserId = transfer.ToUserId,

                ApprovedByUserId = transfer.ApprovedByUserId,
                ApprovedAt = transfer.ApprovedAt,

                Purpose = transfer.Purpose,
                RejectReason = transfer.RejectReason
            };
        }

        public async Task<AssetTransferAllDto> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type)
        {
            var list = await _repo.GetAllTransfer(pageIndex, pageSize, status, type);
            if (list.Items == null || list.TotalCount == 0)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }
            var items = list.Items.Select(h => new AssetTransferResponseDto
            {
                TransferId = h.TransferId,
                AssetId = h.AssetId,
                Asset = new AssetNameDtp
                { 
                    AssetId = h.AssetId,
                    AssetCode = h.Asset.AssetCode,
                    AssetName = h.Asset.AssetName
                },
                TransferType = h.TransferType.ToDisplayName(),
                Status = h.Status.ToDisplayName(),

                DepartmentId=h.DepartmentId,
                Department = h.Department == null ? null : new QuanLyTaiSan.Dtos.Department.DepartmentDto
                { 
                    Id = h.DepartmentId,
                    DepartmentName = h.Department.DepartmentName,
                    Description = h.Department.Description
                },

                FromUserId = h.FromUserId,
                FromUser = h.FromUser == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = h.FromUserId,
                    Username = h.FromUser.UserName,
                    Fullname = h.FromUser.FullName
                },

                ToUserId = h.ToUserId,
                ToUser = h.ToUser == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = h.ToUserId,
                    Username = h.ToUser.UserName,
                    Fullname = h.ToUser.FullName
                },

                CreatedByUserId = h.CreatedByUserId,
                CreatedByUser = h.CreatedByUser == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = h.CreatedByUserId,
                    Username = h.CreatedByUser.UserName,
                    Fullname = h.CreatedByUser.FullName
                },

                ApprovedByUserId = h.ApprovedByUserId,
                ApprovedByUser = h.ApprovedByUser == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                {
                    Id = h.ApprovedByUserId,
                    Username = h.ApprovedByUser.UserName,
                    Fullname = h.ApprovedByUser.FullName
                },
                CreatedAt = h.CreatedAt,
                ApprovedAt =h.ApprovedAt,
                Purpose = h.Purpose,
                Note = h.Note,
                RejectReason = h.RejectReason
            }).ToList();
            var totalPage = (int)Math.Ceiling(list.TotalCount / (double)pageSize);

            return new AssetTransferAllDto
            {
                ListAsset = items,
                Type = type,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = list.TotalCount,
                TotalPage = totalPage,

                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPage
            };
        }

        public async Task<AssetRespondDto> Create(CreateAssetDto createAssetDto)
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
            string currentUserId = GetCurrentUserId();
            await _repo.Create(h);
            await SaveHistory(h, "CREATE", $"Thêm mới tài sản từ loại ID: {h.CategoryId}", currentUserId);
            return new AssetRespondDto
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
        public async Task<AssetRespondDto> Update(UpdateAssetDto updateAssetDto, int id)
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

            //Chặn đổi phòng ban khi đang chờ xác nhận phiếu hoặc có người sở hữu
            bool isChangeDepartment = h.DepartmentId != updateAssetDto.DepartmentId;
            if(isChangeDepartment)
            {
                // Check đang có phiếu bàn giao/thu hồi Pending không
                bool hasPendingTransfer = await _repo.AnyAsync(x => x.AssetId == h.AssetId && x.Status == AssetTransferStatus.Pending);
                if(hasPendingTransfer)
                {
                    throw new InvalidOperationException("Không thể thay đổi phòng ban khi tài sản đang có phiếu bàn giao/thu hồi chờ duyệt");
                }

                // Check tài sản đã có người sử dụng chưa
                if (!string.IsNullOrEmpty(h.UserId))
                {
                    throw new InvalidOperationException("Không thể thay đổi phòng ban khi tài sản đang được cấp cho nhân viên");
                }
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
                string currentUserId = GetCurrentUserId();
                await SaveHistory(h, "UPDATE", "Người dùng cập nhật thông tin tài sản ID : " + id, currentUserId);
            }
            return new AssetRespondDto
            {
                AssetId = h.AssetId,
                AssetCode = h.AssetCode,
                AssetName = h.AssetName,

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

        public async Task Delete(int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có id = " + id);
            }
            if (h.UserId != null)
            {
                throw new InvalidOperationException("Không thể xóa tài sản đã và đang sử dụng bởi người khác");
            }
            bool hasTransfer = await _repo.AnyAsync(t => t.AssetId == id);
            if (hasTransfer)
            { throw new InvalidOperationException("Không thể xóa tài sản đã phát sinh bàn giao hoặc thu hồi"); }

            await SaveHistory(h, "DELETE", "Xóa tài sản khỏi hệ thống", GetCurrentUserId());
            h.IsDelete = true;
            await _repo.Update();
        }

        public async Task<List<AssetRespondDto>> GetAll()
        {
            var lissAsset = await _repo.GetAll();
            if (lissAsset == null)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }
            else
            {
                var list = new List<AssetRespondDto>();
                foreach (var h in lissAsset)
                {
                    string roleName = null;

                    if (h.User != null)
                    {
                        var roles = await _userManager.GetRolesAsync(h.User);
                        roleName = roles.FirstOrDefault();
                    }
                    list.Add(new AssetRespondDto
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
                        UserId = h.UserId,
                        User = h.User == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                        {
                            Id = h.UserId,
                            Username = h.User.UserName,
                            Fullname = h.User.FullName,
                            Role = roleName
                        }
                    });

                }

                return list.ToList();
            }
        }

        public async Task<AssetAllDto> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, string? userId, int? status, string sortBy, bool desc)
        {
            var data = await _repo.GetPageList(pageIndex, pageSize, search, categoryId, userId, status, sortBy, desc);
            if (data.Items == null || data.Items.Count == 0)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }
            var item = new List<AssetRespondDto>();
            foreach(var h in data.Items)
            {
                string roleName = null;

                if (h.User != null)
                {
                    var roles = await _userManager.GetRolesAsync(h.User);
                    roleName = roles.FirstOrDefault();
                }
                item.Add(new AssetRespondDto
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
                    UserId = h.UserId,
                    User = h.User == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto
                    {
                        Id = h.UserId,
                        Username = h.User.UserName,
                        Fullname = h.User.FullName,
                        Role = roleName
                    }
                });
            }    

            var totalPage = (int)Math.Ceiling(data.TotalCount / (double)pageSize);

            return new AssetAllDto
            {
                ListAsset = item,
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

        public async Task<AssetDto> GetById(int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có id = " + id);
            }
            else
            {
                string roleName = null;
                var userEntity = h.User;
                if (h.UserId != null)
                {
                    if (userEntity == null)
                    {
                        userEntity = await _userManager.FindByIdAsync(h.UserId);
                    }

                    if (userEntity != null)
                    {
                        var roles = await _userManager.GetRolesAsync(userEntity);
                        roleName = roles.FirstOrDefault(); 
                    }
                }
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
                    DepartmentId = h.DepartmentId,
                    UserId = h.UserId,
                    Category = new QuanLyTaiSan.Dtos.Category.CategoryDto()
                    {
                        CategoryId = h.CategoryId,
                        CategoryName = h.Category.CategoryName,
                        Description = h.Category.Description
                    },
                    Department = new QuanLyTaiSan.Dtos.Department.DepartmentDto()
                    {
                        Id = h.Department.Id,
                        DepartmentName = h.Department.DepartmentName,
                        Description = h.Department.Description
                    },
                    User = h.UserId == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto()
                    {
                        Id = h.UserId,
                        Username = h.User.UserName,
                        Fullname = h.User.FullName,
                        Role = roleName
                        //Email = h.User.Email,
                        //PhoneNumber = h.User.PhoneNumber
                    }
                };
            }
        }

        public async Task<AssetDto> GetByCode(string code)
        {
            var h = await _repo.GetByCode(code);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản có mã code = " + code);
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
                    CategoryId = h.CategoryId,
                    DepartmentId = h.DepartmentId,
                    UserId = h.UserId,
                    Category = new QuanLyTaiSan.Dtos.Category.CategoryDto()
                    {
                        CategoryId = h.CategoryId,
                        CategoryName = h.Category.CategoryName,
                        Description = h.Category.Description
                    },
                    Department = new QuanLyTaiSan.Dtos.Department.DepartmentDto()
                    {
                        Id = h.Department.Id,
                        DepartmentName = h.Department.DepartmentName,
                        Description = h.Department.Description
                    },
                    User = h.UserId == null ? null : new QuanLyTaiSan.Dtos.Auth.UserDto()
                    {
                        Id = h.UserId,
                        Username = h.User.UserName,
                        Fullname = h.User.FullName
                        //Email = h.User.Email,
                        //PhoneNumber = h.User.PhoneNumber
                    }
                };
            }
        }
        //Hàm xư lý lưu lịch sử
        private async Task SaveHistory(Asset asset, string actionType, string note, string? CreatedByUserId = null, string AssignedToUserId = null)
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
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                return null;

            // UserId trong JWT nằm ở Sub hoặc NameIdentifier
            var userId =
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return userId;
        }
        private string? GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(JwtRegisteredClaimNames.UniqueName);
        }

    }
}
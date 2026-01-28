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
        //Tạo phiếu bàn giao nhiều tài sản
        public async Task<List<AssetFormHandoverDto>> CreateMultiFormHandover(CreateMultiFormTransferDto createMultiFormHandoverDto)
        {
            //Kiểm tra đầu vào null không
            if (createMultiFormHandoverDto == null)
                throw new ArgumentNullException(nameof(createMultiFormHandoverDto));

            if (createMultiFormHandoverDto.AssetIds == null || !createMultiFormHandoverDto.AssetIds.Any())
                throw new ArgumentException("Danh sách tài sản trống");

            if (string.IsNullOrEmpty(createMultiFormHandoverDto.ToUserId))
                throw new ArgumentException("Vui lòng chọn nhân viên ");

            if (createMultiFormHandoverDto.DepartmentId == null)
                throw new ArgumentException("Vui lòng chọn phòng ban ");

            //Kiêm tra tài sản có tồn tại hay không
            var assets = await _repo.GetAssetsByIds(createMultiFormHandoverDto.AssetIds);
            if (assets.Count != createMultiFormHandoverDto.AssetIds.Count) 
                throw new KeyNotFoundException("Một số tài sản không tồn tại");

            //Kiểm tra người dùng có tồn tại hay không
            var user = await _authService.GetUserById(createMultiFormHandoverDto.ToUserId);
            if (user == null)
                throw new KeyNotFoundException($"KNgười nhận không tồn tại");

            //Kiểm tra xem tài khoản người dùng còn hoạt động không
            if (user.Status != UserStatus.active)
                throw new KeyNotFoundException("Tài khoản người nhận đã ngừng hoạt động");

            //Kiểm tra phòng ban có tồn tại hay không
            var department = await _departmentService.GetDepartmentById(createMultiFormHandoverDto.DepartmentId.Value);

            if (department == null)
                throw new KeyNotFoundException("Phòng ban không tồn tại");

            var resultList = new List<AssetFormHandoverDto>();
            var transferList = new List<AssetTransfer>();
            var batchId = Guid.NewGuid();

            foreach (var asset in assets)
            {
                //Check xung đột phiếu
                bool isPending = await _repo.AnyAsync(x => x.AssetId == asset.AssetId && x.Status == AssetTransferStatus.Pending);
                if (isPending)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} đang chờ duyệt");

                //Check xem tài sản đã được bàn giao hay chưa
                if (!string.IsNullOrEmpty(asset.UserId))
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} đang được giữ bởi người khác. Phải thu hồi trước");

                //Check trạng thái tài sản
                if (asset.Status != AssetStatus.SanSang)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} không ở trạng thái sẵn sàng để bàn giao");


                //check xem tài sản , nhân viên có cùng thuộc 1 phòng ban không
                if (asset.DepartmentId != createMultiFormHandoverDto.DepartmentId)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} không thuộc về phòng ban này");
                if (user.DepartmentId != createMultiFormHandoverDto.DepartmentId)
                    throw new InvalidOperationException($"Nhân viên đã chọn không thuộc về phòng ban này");
                if (asset.DepartmentId != user.DepartmentId)
                    throw new InvalidOperationException("Tài sản và nhân viên không cùng thuộc 1 phòng ban");

                //add phiếu vào danh sách
                transferList.Add(new AssetTransfer
                {
                    AssetId = asset.AssetId,
                    TransferType = QuanLyTaiSan.Enum.AssetTransferType.Handover,
                    Status = QuanLyTaiSan.Enum.AssetTransferStatus.Pending,
                    FromDepartmentId = createMultiFormHandoverDto.DepartmentId,
                    ToDepartmentId = null,
                    FromUserId = null,
                    ToUserId = createMultiFormHandoverDto.ToUserId,
                    CreatedByUserId = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    Purpose = createMultiFormHandoverDto.Purpose,
                    Note = createMultiFormHandoverDto.Note,
                    BatchId = batchId
                });
            }

            //Nếu các tài sản đều hợp lệ thì thêm vào db
            foreach (var transfer in transferList)
            {
                await _repo.AddTransfer(transfer);

                // Add vào list kết quả trả về
                resultList.Add(new AssetFormHandoverDto
                {
                    TransferId = transfer.TransferId,
                    AssetId = transfer.AssetId,
                    BatchId = transfer.BatchId,
                    TransferType = transfer.TransferType.ToDisplayName(),
                    Status = transfer.Status.ToDisplayName(),
                    FromDepartmentId = transfer.FromDepartmentId,
                    ToDepartmentId = transfer.ToDepartmentId,
                    FromUserId = transfer.FromUserId,
                    ToUserId = transfer.ToUserId,
                    CreatedByUserId = transfer.CreatedByUserId,
                    Purpose = transfer.Purpose,
                    Note = transfer.Note,
                    CreatedAt = transfer.CreatedAt
                });
            }

            return resultList;
        }

        // Tạo phiếu thu hồi nhiều tài sản
        public async Task<List<AssetFormHandoverDto>> CreateMultiFormRecall(CreateMultiFormTransferDto createMultiFormRecallDto)
        {
            //Kiểm tra đầu vào null không
            if (createMultiFormRecallDto.AssetIds == null || !createMultiFormRecallDto.AssetIds.Any())
                throw new ArgumentException("Danh sách tài sản thu hồi bị trống");
            if (string.IsNullOrEmpty(createMultiFormRecallDto.ToUserId))
                throw new ArgumentException("Vui lòng chọn nhân viên cần thu hồi");

            var user = await _authService.GetUserById(createMultiFormRecallDto.ToUserId);
            if (user == null) throw new KeyNotFoundException("Nhân viên không tồn tại");

            // Lấy dữ liệu tài sản
            var assets = await _repo.GetAssetsByIds(createMultiFormRecallDto.AssetIds);
            if (assets.Count != createMultiFormRecallDto.AssetIds.Count)
                throw new KeyNotFoundException("Một số tài sản không tồn tại hoặc đã bị xóa");


            // Chuẩn bị danh sách
            var resultList = new List<AssetFormHandoverDto>();
            var transferList = new List<AssetTransfer>();
            var batchId = Guid.NewGuid();
            foreach (var asset in assets)
            {
                //Check xung đột phiếu , đang có phiếu treo
                bool isPending = await _repo.AnyAsync(x => x.AssetId == asset.AssetId && x.Status == AssetTransferStatus.Pending);
                if (isPending)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} đang có phiếu chờ duyệt");

                // kiểm tra xem tài sản có đang được sử dụng không 
                if (string.IsNullOrEmpty(asset.UserId))
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} chưa được cấp phát, không thể thu hồi");


                // kiểm tra xem nhân viên có sở hữu  tài sản này không 
                if (asset.UserId != createMultiFormRecallDto.ToUserId)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} không thuộc về nhân viên {user.Fullname}");
                
                //Tạo phiếu
                var transfer = new AssetTransfer
                {
                    AssetId = asset.AssetId,
                    BatchId = batchId,
                    TransferType = QuanLyTaiSan.Enum.AssetTransferType.Recall,
                    Status = QuanLyTaiSan.Enum.AssetTransferStatus.Pending,

                    FromDepartmentId = createMultiFormRecallDto.DepartmentId,
                    ToDepartmentId = createMultiFormRecallDto.DepartmentId,

                    FromUserId = asset.UserId,
                    ToUserId = null,

                    CreatedByUserId = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    Purpose = createMultiFormRecallDto.Purpose,
                    Note = createMultiFormRecallDto.Note
                };
                transferList.Add(transfer);
            }
            //Lưu vào Database
            foreach (var transfer in transferList)
            {
                await _repo.AddTransfer(transfer);
                resultList.Add(new AssetFormHandoverDto
                {
                    TransferId = transfer.TransferId,
                    AssetId = transfer.AssetId,
                    BatchId = transfer.BatchId,
                    TransferType = transfer.TransferType.ToDisplayName(),
                    Status = transfer.Status.ToDisplayName(),
                    FromDepartmentId = transfer.FromDepartmentId,
                    ToDepartmentId = transfer.ToDepartmentId,

                    FromUserId = transfer.FromUserId,
                    ToUserId = transfer.ToUserId,
                    CreatedByUserId = transfer.CreatedByUserId,
                    Note = transfer.Note,
                    Purpose = transfer.Purpose,
                    CreatedAt = transfer.CreatedAt
                });
            }
            return resultList;
        }

        // Tạo phiếu điều chuyển nhiều tài sản sang phòng ban khác
        public async Task<List<AssetFormHandoverDto>> CreateMultiFormDepartmentMove(CreateMultiDepartmentFormTransferDto createMultiDepartmentFormTransferDto)
        {
            //Kiểm tra đầu vào null không
            if (createMultiDepartmentFormTransferDto == null)
                throw new ArgumentNullException(nameof(createMultiDepartmentFormTransferDto));

            if (createMultiDepartmentFormTransferDto.AssetIds == null || !createMultiDepartmentFormTransferDto.AssetIds.Any())
                throw new ArgumentException("Danh sách tài sản điều chuyển bị trống");

            if (createMultiDepartmentFormTransferDto.ToDepartmentId == null)
                throw new ArgumentException("Vui lòng chọn phòng ban nhận bàn giao");

            //Phòng ban nhận phải khác phòng ban ban đầu
            if (createMultiDepartmentFormTransferDto.FromDepartmentId == createMultiDepartmentFormTransferDto.ToDepartmentId)
            {
                throw new InvalidOperationException("Phòng ban chuyển đến phải khác phòng ban hiện tại");
            }

            //Kiểm tra phòng ban nhận có tồn tại hay không
            var department = await _departmentService.GetDepartmentById(createMultiDepartmentFormTransferDto.ToDepartmentId);
            if (department == null)
                throw new KeyNotFoundException("Phòng ban nhận không tồn tại");

            //Kiểm tra xem phòng ban còn hoạt động không
            if (department.DepartmentStatus != DepartmentStatus.Active)
                throw new KeyNotFoundException("Phòng ban nhận đã ngừng hoạt động");

            //Kiêm tra tài sản có tồn tại hay không
            var assets = await _repo.GetAssetsByIds(createMultiDepartmentFormTransferDto.AssetIds);
            if (assets.Count != createMultiDepartmentFormTransferDto.AssetIds.Count)
                throw new KeyNotFoundException("Một số tài sản không tồn tại hoặc đã bị xóa");


            var transferList = new List<AssetTransfer>();
            var resultList = new List<AssetFormHandoverDto>();
            string currentUserId = GetCurrentUserId();
            var batchId = Guid.NewGuid();

            foreach (var asset in assets)
            {
                // Kiểm tra tài sản có thuộc phòng ban nguồn đã chọn không
                if (asset.DepartmentId != createMultiDepartmentFormTransferDto.FromDepartmentId)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} không thuộc phòng ban nguồn bạn đã chọn");

                //Check xung đột phiếu
                bool isPending = await _repo.AnyAsync(x => x.AssetId == asset.AssetId && x.Status == AssetTransferStatus.Pending);
                if (isPending)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} đang có phiếu bàn giao/thu hồi chờ duyệt");

                //Check xem tài sản đã được bàn giao hay chưa
                if (!string.IsNullOrEmpty(asset.UserId))
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode} đang được giữ bởi người khác. Phải thu hồi trước");

                //Kiểm tra tài sản có thuộc phòng ban nào không
                if (asset.DepartmentId == null)
                    throw new InvalidOperationException("Tài sản chưa được gán cho phòng ban nào");


                //Check trạng thái tài sản
                if (asset.Status == AssetStatus.DangSuDung || asset.Status == AssetStatus.ThanhLy || asset.Status == AssetStatus.Mat)
                    throw new InvalidOperationException($"Tài sản {asset.AssetCode}không ở trạng thái có thể bàn giao");

                //Tạo phiếu
                var transfer = new AssetTransfer
                {
                    AssetId = asset.AssetId,
                    BatchId = batchId,
                    TransferType = QuanLyTaiSan.Enum.AssetTransferType.DepartmentMove,
                    Status = QuanLyTaiSan.Enum.AssetTransferStatus.Pending,

                    FromDepartmentId = asset.DepartmentId,
                    ToDepartmentId = createMultiDepartmentFormTransferDto.ToDepartmentId,

                    FromUserId = null,
                    ToUserId = null,

                    CreatedByUserId = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    Purpose = createMultiDepartmentFormTransferDto.Purpose,
                    Note = createMultiDepartmentFormTransferDto.Note
                };
                transferList.Add(transfer);
            }

            foreach (var transfer in transferList)
            {
                await _repo.AddTransfer(transfer);
                resultList.Add(new AssetFormHandoverDto
                {
                    TransferId = transfer.TransferId,
                    AssetId = transfer.AssetId,
                    BatchId = transfer.BatchId,
                    TransferType = transfer.TransferType.ToDisplayName(),
                    Status = transfer.Status.ToDisplayName(),
                    FromDepartmentId = transfer.FromDepartmentId,
                    ToDepartmentId = transfer.ToDepartmentId,
                    FromUserId = transfer.FromUserId,
                    ToUserId = transfer.ToUserId,
                    CreatedByUserId = transfer.CreatedByUserId,
                    Purpose = transfer.Purpose,
                    Note = transfer.Note,
                    CreatedAt = transfer.CreatedAt
                });
            }
            return resultList;
        }


        //Duyệt/Từ chối nhiều phiếu
        public async Task<List<ProcessTransferResultDto>> ProcessMultiApproval(ProcessMultiTransferDto processMultiTransferDto)
        {
            // Lấy tất cả phiếu có cùng BATCH ID
            var transfers = await _repo.GetTransfersByBatchId(processMultiTransferDto.BatchId);
            if (transfers == null || !transfers.Any())
                throw new KeyNotFoundException("Không tìm thấy phiếu để duyệt");
            //Check xem đã duyệt chưa
            if (transfers.First().Status != AssetTransferStatus.Pending)
                throw new InvalidOperationException("Phiếu này đã được xử lý rồi");

            //Kiểm tra người duyệt có tồn tại không
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                throw new UnauthorizedAccessException("Không xác định được người duyệt");


            var results = new List<ProcessTransferResultDto>();

            foreach(var transfer in transfers)
            {
                if (transfer.Status != AssetTransferStatus.Pending)
                {
                    throw new InvalidOperationException($"Phiếu {transfer.TransferId} không ở trong trạng thái chờ duyệt");
                }

                if(transfer.TransferType!=AssetTransferType.Handover && transfer.TransferType != AssetTransferType.Recall && transfer.TransferType != AssetTransferType.DepartmentMove)
                {
                    throw new InvalidOperationException("Loại phiếu không hợp lệ");
                }
            }
            foreach (var transfer in transfers)
            {
                // Trường hợp từ chối
                if (processMultiTransferDto.IsApproved == false)
                {
                    transfer.Status = AssetTransferStatus.Rejected;
                    transfer.ApprovedByUserId = currentUserId;
                    transfer.ApprovedAt = DateTime.Now;
                    transfer.RejectReason = processMultiTransferDto.RejectReason ?? "Từ chối hàng loạt";

                    await SaveHistory(transfer.Asset, "TRANSFER_REJECTED",
                        $"Từ chối phiếu {transfer.TransferId}. Lý do: {transfer.RejectReason}", currentUserId);
                }
                // Trường hợp duyệt phiếu
                else
                {
                    transfer.Status = AssetTransferStatus.Approved;
                    transfer.ApprovedByUserId = currentUserId;
                    transfer.ApprovedAt = DateTime.Now;

                    string? actionType = "";
                    string? note = "";
                    string? assignedUserId = null;
                    if (transfer.TransferType == AssetTransferType.Handover)
                    {
                        transfer.Asset.UserId = transfer.ToUserId;
                        transfer.Asset.Status = AssetStatus.DangSuDung;

                        assignedUserId = transfer.ToUserId;
                        actionType = "HANDOVER";
                        var toUserName = transfer.ToUser?.UserName ?? "Không rõ";
                        note = $"Bàn giao tài sản '{transfer.Asset.AssetCode}-{transfer.Asset.AssetName}' cho nhân viên: {toUserName}";
                    }
                    else if (transfer.TransferType == AssetTransferType.Recall)
                    {
                        transfer.Asset.UserId = null;
                        if (transfer.Asset.Status == AssetStatus.HongHoc)
                        {
                            transfer.Asset.Status = AssetStatus.HongHoc;
                        }
                        else if (transfer.Asset.Status == AssetStatus.ThanhLy)
                        {
                            transfer.Asset.Status = AssetStatus.ThanhLy;
                        }
                        else if (transfer.Asset.Status == AssetStatus.Mat)
                        {
                            transfer.Asset.Status = AssetStatus.Mat;
                        }
                        else
                        {
                            transfer.Asset.Status = AssetStatus.SanSang;
                        }
                        actionType = "RECALL";
                        var fromUserName = transfer.FromUser?.UserName ?? "Đã nghỉ việc/Không rõ";
                        note = $"Thu hồi tài sản '{transfer.Asset.AssetCode}-{transfer.Asset.AssetName}' của nhân viên: {fromUserName}";
                    }
                    else if (transfer.TransferType == AssetTransferType.DepartmentMove)
                    {
                        if (!transfer.ToDepartmentId.HasValue)
                        {
                            throw new InvalidOperationException("Phiếu điều chuyển phòng ban bị thiếu ID phòng ban nhận");
                        }
                        transfer.Asset.UserId = null;
                        transfer.Asset.DepartmentId = transfer.ToDepartmentId.Value;
                        transfer.Asset.Status = AssetStatus.SanSang;

                        actionType = "TRANSFER_DEPARTMENT";
                        var fromDep = transfer.FromDepartment?.DepartmentName ?? "Không rõ";
                        var toDep = transfer.ToDepartment?.DepartmentName ?? "Không rõ";
                        note = $"Điều chuyển tài sản '{transfer.Asset.AssetCode}-{transfer.Asset.AssetName}' từ phòng ban: {transfer.FromDepartment.DepartmentName ?? "Không rõ"} -> {transfer.ToDepartment.DepartmentName ?? "Không rõ"}";
                    }
                    transfer.Asset.UpdatedTime = DateTime.UtcNow;
                    transfer.Status = AssetTransferStatus.Completed;
                    await SaveHistory(transfer.Asset, actionType, note, currentUserId, assignedUserId);
                }
                results.Add(BuildResultDto(transfer));
            }
            await _repo.Update();
            return results;
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

                FromDepartmentId = transfer.FromDepartmentId,
                ToDepartmentId = transfer.ToDepartmentId,

                FromUserId = transfer.FromUserId,
                ToUserId = transfer.ToUserId,

                ApprovedByUserId = transfer.ApprovedByUserId,
                ApprovedAt = transfer.ApprovedAt,

                Purpose = transfer.Purpose,
                RejectReason = transfer.RejectReason
            };
        }
        
        //Lấy tất cả phiếu có phân trang
        public async Task<AssetTransferAllDto> GetAllTransfer(int pageIndex, int pageSize, int? status, int? type)
        {
            // Gọi Repo lấy về danh sách dữ liệu của các lô
            var result = await _repo.GetAllTransfer(pageIndex, pageSize, status, type);
            // Kiểm tra rỗng
            if (result.Items == null || !result.Items.Any())
                return new AssetTransferAllDto { ListBatches = new List<AssetTransferBatchResponseDto>(), TotalCount = 0 };

            //Gom nhóm theo batchId
            var groupedBatches = result.Items.GroupBy(x => x.BatchId).Select(g =>
            {
                var firstItem = g.First(); // Lấy item đầu để lấy thông tin chung

                return new AssetTransferBatchResponseDto
                {
                    BatchId = g.Key,
                    CreatedAt = firstItem.CreatedAt,
                    Status = firstItem.Status.ToDisplayName(),
                    TransferType = firstItem.TransferType.ToDisplayName(),

                    // Map thông tin đại diện
                    FromUserName = firstItem.FromUser?.FullName,
                    ToUserName = firstItem.ToUser?.FullName,
                    FromDepartmentName = firstItem.FromDepartment?.DepartmentName,
                    ToDepartmentName = firstItem.ToDepartment?.DepartmentName,
                    Purpose = firstItem.Purpose,
                    Note = firstItem.Note,
                    AssetCount = g.Count(),

                    // Map danh sách con
                    AssetsTransfer = g.Select(a => new AssetTransferDetailDto
                    {
                        TransferId = a.TransferId,
                        AssetId = a.AssetId,
                        AssetCode = a.Asset.AssetCode,
                        AssetName = a.Asset.AssetName,
                        AssetStatus = a.Asset.Status.ToDisplayName()
                    }).ToList()
                };
            }).OrderByDescending(x => x.CreatedAt) 
              .ToList();

            var totalPage = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

            return new AssetTransferAllDto
            {
                ListBatches = groupedBatches,
                Type = type,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                TotalPage = totalPage,

                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPage
            };

        }

        //Thêm tài sản
        public async Task<AssetRespondDto> Create(CreateAssetDto createAssetDto)
        {
            if (createAssetDto == null)
            {
                throw new BadHttpRequestException("Dữ liệu gửi lên bị null");
            }

            var category = await _repoCate.GetById(createAssetDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Loại tài sản không tồn tại");
            }
            if(category.Status == CategoryStatus.NgungSuDung)
            {
                throw new InvalidOperationException("Loại tài sản đã ngưng sử dụng");
            }

            var department = await _departmentService.GetDepartmentById(createAssetDto.DepartmentId);
            if(department == null)
            {
                throw new KeyNotFoundException("Phòng ban không tồn tại");

            }
            if (department.DepartmentStatus == DepartmentStatus.Inactive)
            {
                throw new InvalidOperationException("Phòng ban đã ngưng sử dụng");
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
            await SaveHistory(h, "CREATE", $"Thêm mới tài sản thuộc loại: {h.Category.CategoryName}", currentUserId);
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

        //Sửa tài sản
        public async Task<AssetRespondDto> Update(UpdateAssetDto updateAssetDto, int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Tài sản không tồn tại");
            }
            if ((await _repoCate.GetById(updateAssetDto.CategoryId)) == null)
            {
                throw new KeyNotFoundException("Loại tài sản không tồn tại");
            }
         
            // So sánh để tạo log
            var changes = new List<string>();

            if (h.AssetName != updateAssetDto.AssetName)
                changes.Add($"Tên: '{h.AssetName}' -> '{updateAssetDto.AssetName}'");

            if (h.Descriptions != updateAssetDto.Descriptions)
                changes.Add($"Mô tả thay đổi");

            // So sánh giá tiền 
            if (h.OriginalValue != updateAssetDto.OriginalValue)
                changes.Add($"Nguyên giá: {h.OriginalValue:N0} -> {updateAssetDto.OriginalValue:N0}");

            // So sánh ngày tháng
            if (h.PurchaseDate != updateAssetDto.PurchaseDate)
                changes.Add($"Ngày mua: {h.PurchaseDate:dd/MM/yyyy} -> {updateAssetDto.PurchaseDate:dd/MM/yyyy}");

            if (h.ManufactureYear != updateAssetDto.ManufactureYear)
                changes.Add($"Năm SX: {h.ManufactureYear} -> {updateAssetDto.ManufactureYear}");

            if ((int)h.Status != updateAssetDto.Status)
                changes.Add($"Trạng thái thay đổi "); 

            if (h.Note != updateAssetDto.Note)
                changes.Add($"Ghi chú thay đổi");

            if (h.Unit != updateAssetDto.Unit)
                changes.Add($"Đơn vị tính: {h.Unit} -> {updateAssetDto.Unit}");

            if (h.CategoryId != updateAssetDto.CategoryId)
                changes.Add($"Loại tài sản ID: {h.CategoryId} -> {updateAssetDto.CategoryId}");

            // Tạo chuỗi nội dung log
            string historyAction = changes.Count > 0
                ? "Cập nhật các trường: " + string.Join("; ", changes)
                : "Cập nhật thông tin (Không có thay đổi thực tế)";
            if ((AssetStatus)updateAssetDto.Status == AssetStatus.ThanhLy)
            {
                // Kiểm tra có ai đang giữ không (UserId phải null hoặc rỗng)
                if (!string.IsNullOrEmpty(h.UserId))
                {
                    throw new InvalidOperationException($"Tài sản đang được giữ bởi nhân viên ({h.User.FullName}). Vui lòng thu hồi tài sản về kho trước khi thanh lý.");
                }

                // Kiểm tra trạng thái hiện tại (Chỉ được thanh lý khi đang Hỏng hoặc Sẵn sàng)
                // Không thể thanh lý khi đang "Đang sử dụng", "Mất", hoặc "Đã thanh lý" rồi.
                if (h.Status != AssetStatus.HongHoc && h.Status != AssetStatus.SanSang)
                {
                    throw new InvalidOperationException($"Không thể thanh lý tài sản đang ở trạng thái '{h.Status.ToDisplayName()}'. Tài sản phải ở trạng thái 'Hỏng' hoặc 'Sẵn sàng'.");
                }
            }
            //  Thực hiện Update (
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

            await _repo.Update();

            // Ghi lịch sử với nội dung chi tiết đã tạo ở trên
            string currentUserId = GetCurrentUserId();
            await SaveHistory(h, "UPDATE", historyAction, currentUserId);

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

        //Xoắ tài sản
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

        //Lấy tất cả tài sản
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

        //Lấy tất cả tài sản có phân trang
        public async Task<AssetAllDto> GetPageList(int pageIndex, int pageSize, string? search, int? categoryId, string? userId, int? departmentId, int? status, string sortBy, bool desc)
        {
            var data = await _repo.GetPageList(pageIndex, pageSize, search, categoryId, userId,departmentId, status, sortBy, desc);
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

        //Lấy tài sản theo id
        public async Task<AssetDto> GetById(int id)
        {
            var h = await _repo.GetById(id);
            if (h == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản " );
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

        //Lấy tài sản theo mã code
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
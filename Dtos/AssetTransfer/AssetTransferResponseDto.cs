using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class AssetTransferResponseDto
    {
        public int TransferId { get; set; }
        public int AssetId { get; set; }
        public AssetNameDtp Asset { get; set; }
        public string TransferType { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public string? FromUserId { get; set; }
        public UserDto? FromUser { get; set; }

        public string? ToUserId { get; set; }
        public UserDto? ToUser { get; set; }

        public string Status { get; set; }

        public string CreatedByUserId { get; set; }
        public UserDto CreatedByUser { get; set; }

        public string? ApprovedByUserId { get; set; }
        public UserDto? ApprovedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public string? Note { get; set; }
        public string? Purpose { get; set; }
    }
}

using QuanLyTaiSan.Enum;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSan.Models
{
    public class AssetTransfer
    {
        public int TransferId { get; set; }
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public AssetTransferType TransferType { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string? FromUserId { get; set; }
        public ApplicationUser? FromUser { get; set; }

        public string? ToUserId { get; set; }
        public ApplicationUser? ToUser { get; set; }

        public AssetTransferStatus Status { get; set; }

        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }

        public string? ApprovedByUserId { get; set; }
        public ApplicationUser? ApprovedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public string? Note { get; set; }
        public string? Purpose { get; set; }
    }
}

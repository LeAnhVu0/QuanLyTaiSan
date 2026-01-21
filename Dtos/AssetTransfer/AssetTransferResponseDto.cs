using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class AssetTransferResponseDto
    {
        public int TransferId { get; set; }
        public int AssetId { get; set; }
        public string TransferType { get; set; }
        public string Status { get; set; }


        public string? FromUserId { get; set; }

        public string? ToUserId { get; set; }


        public string CreatedByUserId { get; set; }

        public string? ApprovedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public string? Note { get; set; }
    }
}

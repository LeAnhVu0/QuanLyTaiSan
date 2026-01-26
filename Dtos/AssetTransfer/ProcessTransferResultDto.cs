namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class ProcessTransferResultDto
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; } // HANDOVER / RECALL
        public string Status { get; set; }       // Approved / Rejected / Completed

        public int AssetId { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }

        public int? FromDepartmentId { get; set; }
        public int? ToDepartmentId { get; set; }
        public string? FromUserId { get; set; }
        public string? ToUserId { get; set; }

        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Purpose { get; set; }
        public string? RejectReason { get; set; }
    }
}

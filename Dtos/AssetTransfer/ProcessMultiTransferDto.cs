namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class ProcessMultiTransferDto
    {
        public Guid BatchId { get; set; } 
        public bool IsApproved { get; set; }       // True: Duyệt, False: Từ chối
        public string? RejectReason { get; set; }  // Lý do từ chối (nếu có)
    }
}

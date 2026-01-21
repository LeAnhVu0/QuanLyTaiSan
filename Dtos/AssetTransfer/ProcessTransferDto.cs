namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class ProcessTransferDto
    {
        public bool IsApproved { get; set; }   // true = duyệt | false = từ chối
        public string? RejectReason { get; set; }   
    }
}

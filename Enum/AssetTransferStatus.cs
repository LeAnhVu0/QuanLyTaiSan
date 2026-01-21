using QuanLyTaiSanTest.Enum;

namespace QuanLyTaiSan.Enum
{
    public enum AssetTransferStatus
    {
        Pending = 1,     // Chờ xác nhận
        Approved = 2,    // Đã xác nhận
        Rejected = 3,    // Từ chối
        Completed = 4,    // Thành công
    }
    public static class AssetTransferExtention
    {
        public static string ToDisplayName(this AssetTransferStatus s)
        {
            return s switch
            {
                AssetTransferStatus.Pending => "Chờ xác nhận",
                AssetTransferStatus.Approved => "Đang xác nhận",
                AssetTransferStatus.Rejected => "Từ chối",
                AssetTransferStatus.Completed => "Xác nhận thành công",
                _ => "Không xác định"
            };
        }
    }
}

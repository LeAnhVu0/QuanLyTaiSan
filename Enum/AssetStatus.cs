namespace QuanLyTaiSanTest.Enum
{
    public enum AssetStatus
    {
        SanSang = 1,      // Sẵn sàng sử dụng
        DangSuDung = 2,   // Đang được bàn giao
        HongHoc = 3,      // Đang hỏng/Sửa chữa
        ThanhLy = 4,      // Đã thanh lý
        Mat = 0           // Đã mất
    }
    public static class AssetStatusExtensions
    {
        public static string ToDisplayName(this AssetStatus s)
        {
            return s switch
            {
                AssetStatus.SanSang => "Sẵn sàng",
                AssetStatus.DangSuDung => "Đang sử dụng",
                AssetStatus.HongHoc => "Hỏng hóc",
                AssetStatus.ThanhLy => "Thanh lý",
                AssetStatus.Mat => "Mất",
                _ => "Không xác định"
            };
        }
    }
}

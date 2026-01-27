using QuanLyTaiSanTest.Enum;

namespace QuanLyTaiSan.Enum
{
    public enum InventoryStatus
    {
        ChuaKiemKe = 1,      
        ChenhLech = 2,   
        KhopSoLuong = 3,      
    }
    public static class InventoryStatusExtensions
    {
        public static string ToDisplayName(this InventoryStatus s)
        {
            return s switch
            {
                InventoryStatus.ChuaKiemKe => "Chưa kiểm kê",
                InventoryStatus.ChenhLech => "Chênh lệch",
                InventoryStatus.KhopSoLuong => "Khớp số liệu",
                _ => "Không xác định"
            };
        }
    }
}

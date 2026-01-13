namespace QuanLyTaiSanTest.Enum
{
    public enum CategoryStatus
    {
        DangSuDung = 1,
        NgungSuDung = 2
    }
    public static class CategoryStatusExtensions
    {
        public static string ToDisplayName(this CategoryStatus s)
        {
            return s switch
            {
                CategoryStatus.DangSuDung => "Đang sử dụng",
                CategoryStatus.NgungSuDung => "Ngừng sử dụng",
                _ => "Không xác định"
            };
        }
    }
}

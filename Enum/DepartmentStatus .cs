namespace QuanLyTaiSan.Enum
{
    public enum DepartmentStatus
    {
         Active = 1,      
         Inactive = 2,   
         Deleted = 3 
    }
    public static class DepartmentStatusExtensions
    {
        public static string ToDisplayName(this DepartmentStatus s)
        {
            return s switch
            {
                DepartmentStatus.Active => "Đang sử dụng",
                DepartmentStatus.Inactive => "Ngừng sử dụng",
                DepartmentStatus.Deleted => "Đã xóa",
                _ => "Không xác định"
            };
        }
    }
}

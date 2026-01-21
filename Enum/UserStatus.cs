namespace QuanLyTaiSan.Enum
{
    public enum UserStatus
    {
        active = 1,
        inactive = 0
    }
    public static class UserStatusExtensions
    {
        public static string ToFriendlyString(this UserStatus status) => status switch
        {
            UserStatus.active => "Active",
            UserStatus.inactive => "Inactive",
            _ => "Unknown"
        };
    }
}

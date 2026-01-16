namespace QuanLyTaiSan.Dtos.Auth
{
    public class GrantPermissionsDto
    {
        public string UserId { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}

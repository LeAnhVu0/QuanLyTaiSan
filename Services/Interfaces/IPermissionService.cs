namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IPermissionService
    {
        Task GrantPermissionAsync(string userId, string permission);
        Task RevokePermissionAsync(string userId, string permission);
        Task GrantPermissionsAsync(string userId, List<string> permissions);
        Task<List<string>> GetAdminPermissionsAsync(string userId);
    }
}

using Microsoft.AspNetCore.Identity;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyTaiSan.Services.Implementations
{
    public class PermissionService: IPermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task GrantPermissionAsync(string userId, string permission)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                throw new Exception("User is not Admin");

            var claims = await _userManager.GetClaimsAsync(user);

            if (claims.Any(c => c.Type == "Permission" && c.Value == permission))
                return;

            await _userManager.AddClaimAsync(user,
                new Claim("Permission", permission));
        }
        public async Task RevokePermissionAsync(string userId, string permission)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.FirstOrDefault(c =>
                c.Type == "Permission" && c.Value == permission);

            if (claim != null)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }
        }
        public async Task GrantPermissionsAsync(
            string userId,
            List<string> permissions)
        {
            foreach (var permission in permissions)
            {
                await GrantPermissionAsync(userId, permission);
            }
        }

        public async Task<List<string>> GetAdminPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            var claims = await _userManager.GetClaimsAsync(user);

            return claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();
        }
    }
}

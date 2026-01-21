using Microsoft.AspNetCore.Identity;
using QuanLyTaiSan.Models;
using System.Security.Claims;

namespace QuanLyTaiSan.Data
{

    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

          
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

          
            var superAdminEmail = "Nhat0903@gmail.com";
            var superAdminPassword = "Nhat0902@";

            var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    UserName = "Nhat",
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(superAdmin, superAdminPassword);
                await userManager.AddToRoleAsync(superAdmin, "Admin");
            }

            
            var superAdminPermissions = new[]
            {
        Permissions.GrantPermission,   
        Permissions.AssetCreate,
        Permissions.AssetAssign,
        Permissions.AssetDelete,
        Permissions.AssetUpdate,
        Permissions.AssetGetHistory,
        Permissions.AssetRecall,
        Permissions.UserGet,
        Permissions.UserCreate,
        Permissions.UserDelete,
        Permissions.UserRefreshToken,
        Permissions.DepartmentGet,
        Permissions.DepartmentCreate,
        Permissions.DepartmentUpdate,
        Permissions.DepartmentDelete,
        Permissions.InventoryCreate,
        Permissions.InvetoryUpdate,
        Permissions.ReportGet
    };

            var existingClaims = await userManager.GetClaimsAsync(superAdmin);

            foreach (var permission in superAdminPermissions)
            {
                if (!existingClaims.Any(c =>
                    c.Type == "Permission" && c.Value == permission))
                {
                    await userManager.AddClaimAsync(
                        superAdmin,
                        new Claim("Permission", permission)
                    );
                }
            }
        }

    }

}

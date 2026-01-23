using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Interfaces;
using System.Runtime.InteropServices;

namespace QuanLyTaiSan.Controllers
{
    [ApiController]
    [Route("api/admin/permissions")]
    [Authorize(Policy = Permissions.GrantPermission)]
    public class AdminPermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public AdminPermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        
        [HttpPost("grant-permission")]
        public async Task<IActionResult> GrantPermission(
            GrantPermissionDto dto)
        {
            await _permissionService
                .GrantPermissionAsync(dto.UserId, dto.Permission);

            return Ok("Them quyen thanh cong");
        }

      
        [HttpPost("revoke-permission")]
        public async Task<IActionResult> RevokePermission(
            GrantPermissionDto dto)
        {
            await _permissionService
                .RevokePermissionAsync(dto.UserId, dto.Permission);

            return Ok("Xoa quyen thanh cong");
        }

   
        [HttpPost("grant-permissions")]
        public async Task<IActionResult> GrantPermissions(
            GrantPermissionsDto dto)
        {
            await _permissionService
                .GrantPermissionsAsync(dto.UserId, dto.Permissions);

            return Ok("Them quyen thanh cong");
        }

     
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAdminPermissions(string userId)
        {
            var permissions =
                await _permissionService.GetAdminPermissionsAsync(userId);
            return Ok(permissions);
        }
    }

}

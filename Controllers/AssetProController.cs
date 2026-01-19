using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetProController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetProController(IAssetService assetService) 
        {
            _assetService = assetService;
        }
        [Authorize(Policy = Permissions.AssetAssign)]
        [HttpPost("Handover")]
        public async Task<IActionResult> Handover(int assetId, string userId)
        {
            try
            {
                var result = await _assetService.AssetHandover(assetId, userId);

                return Ok(new ApiResponse<AssetHandoverDto>
                {
                    Success = true,
                    Message = "Bàn giao tài sản thành công",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new { AssetId = assetId, UserId = userId }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lỗi hệ thống",
                    Errors = new { Detail = ex.Message }
                });
            }
        }

        [Authorize(Policy = Permissions.AssetRecall)]
        [HttpPut("Recall/{assetId}")]
        public async Task<IActionResult> AssetRecall(int assetId)
        {
            try
            {
                await _assetService.AssetRecall(assetId);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Thu hồi tài sản thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lỗi hệ thống",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
    }
}

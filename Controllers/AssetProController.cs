using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPut("Handover")]
        public async Task<IActionResult> AssetHandover(int assetId , string userId)
        {
            try
            {
                var result = await _assetService.AssetHandover(assetId, userId);

                return Ok(new { message = "Bàn giao tài sản thành công", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPut("Recall/{assetId}")]
        public async Task<IActionResult> AssetRecall(int assetId)
        {
            try
            {
                await _assetService.AssetRecall(assetId);
                return Ok(new { message = "Thu hồi tài sản về kho thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}

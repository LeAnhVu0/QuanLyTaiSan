using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.AssetTransfer;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Globalization;

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
        [HttpGet("ListTransfer")]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 5, int? status = null,int? type = null)
        {
            try
            {
                var list = await _assetService.GetAllTransfer(pageIndex,pageSize,status,type);

                return Ok(new ApiResponse<List<AssetTransferResponseDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách thành công",
                    Data = list
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy danh sách thất bại",
                    Errors = new { Detail = ex.Message }
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


        [Authorize(Policy = Permissions.AssetAssign)]
        [HttpPost("CreateFormHandover")]
        public async Task<IActionResult> CreateFormTransfer(CreateFormTransferDto createFormTransferDto)
        {
            try
            {
                var form = await _assetService.CreateFormHandover(createFormTransferDto);

                return Ok(new ApiResponse<AssetFormHandoverDto>
                {
                    Success = true,
                    Message = "Tạo phiếu bàn giao thành công",
                    Data = form
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu bàn giao thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Tạo phiếu bàn giao thất bại",
                    Errors = new { Detail = ex.Message }
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
        [HttpPost("CreateFormRecall")]
        public async Task<IActionResult> CreateFormRecall(CreateFormTransferDto createFormTransferDto)
        {
            try
            {
                var form = await _assetService.CreateFormRecall(createFormTransferDto);

                return Ok(new ApiResponse<AssetFormHandoverDto>
                {
                    Success = true,
                    Message = "Tạo phiếu thu hồi thành công",
                    Data = form
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu thu hồi thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Tạo phiếu thu hồi thất bại",
                    Errors = new { Detail = ex.Message }
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
        [HttpPost("ProcessApproval")]
        public async Task<IActionResult> ProcessApproval(int transfer,ProcessTransferDto processTransferDto)
        {
            try
            {
                var result = await _assetService.ProcessApproval(transfer,processTransferDto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = processTransferDto.IsApproved ? "Duyệt phiếu thành công":"Từ chối duyệt phiếu",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Xác nhận phiếu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Xác nhận phiếu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Xác nhận phiếu thất bại",
                    Errors = new { Detail = ex.Message }
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

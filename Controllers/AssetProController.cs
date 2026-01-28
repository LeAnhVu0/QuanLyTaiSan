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

                return Ok(new ApiResponse<AssetTransferAllDto>
                {
                    Success = true,
                    Message = "Lấy danh sách thành công",
                    Data = list
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
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
        [HttpPost("CreateFormHandovers")]
        public async Task<IActionResult> CreateFormTransfers(CreateMultiFormTransferDto createMultiFormHandoverDto)
        {
            try
            {
                var form = await _assetService.CreateMultiFormHandover(createMultiFormHandoverDto);

                return Ok(new ApiResponse<List<AssetFormHandoverDto>>
                {
                    Success = true,
                    Message = "Tạo phiếu bàn giao thành công",
                    Data = form
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu bàn giao thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (ArgumentException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu bàn giao thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
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
        [HttpPost("CreateFormRecalls")]
        public async Task<IActionResult> CreateFormRecalls(CreateMultiFormTransferDto createMultiFormRecallDto)
        {
            try
            {
                var form = await _assetService.CreateMultiFormRecall(createMultiFormRecallDto);

                return Ok(new ApiResponse<List<AssetFormHandoverDto>>
                {
                    Success = true,
                    Message = "Tạo phiếu thu hồi thành công",
                    Data = form
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu thu hồi thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
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

        [HttpPost("CreateDepartmentFormTransfers")]
        public async Task<IActionResult> CreateDepartmentFormTransfers(CreateMultiDepartmentFormTransferDto createMultiDepartmentFormTransferDto)
        {
            try
            {
                var form = await _assetService.CreateMultiFormDepartmentMove(createMultiDepartmentFormTransferDto);

                return Ok(new ApiResponse<List<AssetFormHandoverDto>>
                {
                    Success = true,
                    Message = "Tạo phiếu điều chuyển thành công",
                    Data = form
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu điều chuyển thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (ArgumentException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Tạo phiếu điều chuyển thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Tạo phiếu điều chuyển thất bại",
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

        [HttpPost("ProcessApprovals")]
        public async Task<IActionResult> ProcessApprovals(ProcessMultiTransferDto processMultiTransferDto)
        {
            try
            {
                var result = await _assetService.ProcessMultiApproval(processMultiTransferDto);

                return Ok(new ApiResponse<List<ProcessTransferResultDto>>
                {
                    Success = true,
                    Message = processMultiTransferDto.IsApproved ? "Duyệt phiếu thành công" : "Từ chối duyệt phiếu",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Xác nhận phiếu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
                {

                    Success = false,
                    Message = "Xác nhận phiếu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Ok(new ApiResponse<string>
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

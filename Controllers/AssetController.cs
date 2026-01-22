using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IAssetHistoryService _assetHistoryService;

        public AssetController(IAssetService assetService,IAssetHistoryService assetHistoryService)
        {
            _assetService = assetService;
            _assetHistoryService = assetHistoryService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var asset = await _assetService.GetAll();
                return Ok(new ApiResponse<List<AssetRespondDto>>
                {
                    Success = true,
                    Message = "Lấy tài sản thành công",
                    Data = asset
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy dữ liệu thất bại",
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


        [HttpGet("GetPageList")]
        public async Task<IActionResult> GetPageList(int pageIndex = 1, int pageSize = 5, string? searchName = null, int? status = null, int? categoryId = null, string sortBy = "date", bool desc=true )
        {
           
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                if(pageSize < 1)
                {
                    pageSize = 5;
                }
                return Ok( await _assetService.GetPageList(pageIndex, pageSize, searchName, categoryId, status, sortBy, desc));
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy dữ liệu thất bại",
                    Errors = new { Detail = ex.Message}
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
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var asset = await _assetService.GetById(id);
                return Ok(new ApiResponse<AssetDto>
                {
                    Success = true,
                    Message = "Lấy tài sản thành công",
                    Data = asset
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy dữ liệu thất bại",
                    Errors = new {Detail= ex.Message }
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

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var asset = await _assetService.GetByCode(code);
                return Ok(new ApiResponse<AssetDto>
                {
                    Success = true,
                    Message = "Lấy tài sản thành công",
                    Data = asset
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy dữ liệu thất bại",
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

        [Authorize(Policy = Permissions.UserCreate)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAssetDto createAssetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var asset = await _assetService.Create(createAssetDto);

                return Ok(new ApiResponse<AssetRespondDto> 
                {
                    Success = true,
                    Message = "Thêm tài sản thành công",
                    Data = asset
                    
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Thêm tài sản thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Thêm tài sản  thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lỗi hệ thống",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
        [Authorize(Policy = Permissions.AssetDelete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id không hợp lệ");
            }
            try
            {
                await _assetService.Delete(id);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Xóa tài sản thành công",
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Xóa tài sản thất bại",
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
        [Authorize(Policy = Permissions.AssetUpdate)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id,UpdateAssetDto updateAssetDto)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success=false,
                    Message = "Id không hợp lệ"
                });
            }
            try
            {
                var asset = await _assetService.Update(updateAssetDto,id);
                return Ok(new ApiResponse<AssetRespondDto>
                {
                    Success=true,
                    Message= "Sửa tài sản thành công",
                    Data = asset
                    
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Sửa tài sản thất bại",
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
        [Authorize(Policy = Permissions.AssetGetHistory)]
        [HttpGet("History")]
        public async Task<IActionResult> GetAllHistory()
        {
            try
            {
                return Ok( await _assetHistoryService.GetAll());
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lấy dữ liệu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
    }
}

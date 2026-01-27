using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Category;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSan.Dtos.Inventory;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Services.Implementations;
using QuanLyTaiSanTest.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyTaiSanTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvention(int pageIndex = 1, int pageSize = 5, int? departmentId = null, int? status = null)
        {
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                if (pageSize < 1)
                {
                    pageSize = 5;
                }
                var data = await _inventoryService.GetAll(pageIndex, pageSize, departmentId, status);

                return Ok(new ApiResponse<InventoryAllDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Hiển thị thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Lấy dữ liệu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hiển thị thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            try
            {
                var inventory = await _inventoryService.GetById(id);
                return Ok(new ApiResponse<InventoryResponseDto>
                {
                    Success = true,
                    Message = "Lấy phiếu kiểm kê thành công",
                    Data = inventory
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


        [Authorize(Policy = Permissions.InventoryCreate)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryDto createInventoryDto)
        {
            try
            {
                var data = await _inventoryService.CreatePlan(createInventoryDto);

                return Ok(new ApiResponse<CreateInventoryResponseDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Hiển thị thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hiển thị thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
        [Authorize(Policy = Permissions.DepartmentUpdate)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateInventoryDto  updateInventoryDto)
        {
            try
            {
                var data = await _inventoryService.Update(id , updateInventoryDto);

                return Ok(new ApiResponse<InventoryResponseDto>
                {
                    Success = true,
                    Data = data,
                    Message = "Hiển thị thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Lấy dữ liệu thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hiển thị thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
        }
    }
}

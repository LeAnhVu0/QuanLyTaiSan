using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSan.Dtos.Inventory;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.NewFolder1;
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
        public async Task<IActionResult> GetAllInvention(int pageIndex = 1, int pageSize = 5)
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
                var data = await _inventoryService.GetAll(pageIndex,pageSize);

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
                    Success = false,
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

        [Authorize(Policy = Permissions.InventoryCreate)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryDto createInventoryDto)
        {
            try
            {
                return Ok(await _inventoryService.CreatePlan(createInventoryDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = Permissions.DepartmentUpdate)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateInventoryDto  updateInventoryDto)
        {
            try
            {
                await _inventoryService.Update(id, updateInventoryDto);
                return Ok("Sua thanh cong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

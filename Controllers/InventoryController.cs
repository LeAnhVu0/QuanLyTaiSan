using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSanTest.Dtos.NewFolder1;
using QuanLyTaiSanTest.Services.Interfaces;

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

using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Services.Interfaces;

namespace QuanLyTaiSan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var result = await _service.GetDepartmentById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] DepartmentCreateDto dto)
        {
            var result = await _service.AddDepartment(dto);
            return Ok(result);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            var result = await _service.UpdateDepartment(id, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDepartment(int id)
        {
            await _service.DeleteDepartment(id);
            return Ok("Xoa thanh cong");
        }
    }
}

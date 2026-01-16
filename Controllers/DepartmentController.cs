using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Implementations;
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
        [Authorize(Policy = Permissions.DepartmentGet)]
        [HttpGet]
        public async Task<IActionResult> GetDepartments(
    int pageIndex = 1,
    int pageSize = 5)
        {
            return Ok(
                await _service.GetDepartmentsAsync(pageIndex, pageSize)
            );
        }
        [Authorize(Policy = Permissions.DepartmentGet)]
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var result = await _service.GetDepartmentById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [Authorize(Policy = Permissions.DepartmentCreate)]
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] DepartmentCreateDto dto)
        {
            var result = await _service.AddDepartment(dto);
            return Ok(result);
        }
        [Authorize(Policy = Permissions.DepartmentUpdate)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, DepartmentUpdateDto dto)
        {
            var result = await _service.UpdateDepartment(id, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [Authorize(Policy = Permissions.DepartmentDelete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDepartment(int id)
        {
            await _service.DeleteDepartment(id);
            return Ok("Xoa thanh cong");
        }
    }
}

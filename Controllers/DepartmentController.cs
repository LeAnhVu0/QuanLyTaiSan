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
        public async Task<ActionResult<DepartmentResponseDto>> GetById(int id)
        {
            var result = await _service.GetDepartmentById(id);
            if (result == null)
                return NotFound("Lấy phòng ban k thành công");
            return Ok(result);
        }
        [Authorize(Policy = Permissions.DepartmentCreate)]
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<DepartmentResponseDto>> CreateDepartment(
    [FromBody] DepartmentCreateDto dto)
        {
            try
            {
                var result = await _service.AddDepartment(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Policy = Permissions.DepartmentUpdate)]
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentResponseDto>> UpdateDepartment(
    int id, DepartmentUpdateDto dto)
        {
            try
            {
                var result = await _service.UpdateDepartment(id, dto);
                if (result == null)
                    return NotFound("update k thanh cong");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Policy = Permissions.DepartmentDelete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDepartment(int id)
        {
            var result =await _service.DeleteDepartment(id);
            if (result == null)
                return NotFound("Department not found");

            if (result == "Cannot delete department with assigned users")
                return BadRequest(result);
            return Ok("Xoa thanh cong");
        }
    }
}

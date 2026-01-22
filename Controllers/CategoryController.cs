using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Common;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Services.Implementations;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryServicecs) 
        {
            _categoryService = categoryServicecs;
        }
        [HttpGet("GetPageList")]
        public async Task<IActionResult> GetPageList(int pageIndex = 1, int pageSize=5, string? search=null, int? status = null, string sortBy = "name", bool desc = true)
        {
            try
            {
                return Ok(await _categoryService.GetPageList(pageIndex, pageSize, search, status, sortBy, desc));
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
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                var listCategory = await _categoryService.GetAll();
                return Ok(new ApiResponse<List<CategoryResponseDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách thành công",
                    Data = listCategory
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            
            try
            {
                var category = await _categoryService.GetById(id);
                return Ok(new ApiResponse<CategoryResponseDto>
                {
                    Success = true,
                    Message = "Lấy tài sản thành công",
                    Data = category
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
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _categoryService.Create(createCategoryDto);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Thêm loại tài sản thành công",

                });
            }
            catch (BadHttpRequestException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Thêm loại tài sản thất bại",
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateCategoryDto updateCategoryDto, int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Id không hợp lệ"
                });
            }
            try
            {
                await _categoryService.Update(updateCategoryDto, id);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Sửa loại tài sản thành công",

                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Sửa loại tài sản thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Sửa loại tài sản thất bại",
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id không hợp lệ");
            }
            try
            {
                await _categoryService.Delete(id);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Xóa loại tài sản thành công",
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Xóa loại tài sản thất bại",
                    Errors = new { Detail = ex.Message }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Xóa loại tài sản thất bại",
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSanTest.Dtos.Category;
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
        [HttpGet]
        public async Task<IActionResult> GetAll(string? search, string sortBy = "name", bool desc = true)
        {
            try
            {
                return Ok(await _categoryService.GetAll(search, sortBy, desc));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _categoryService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
        {
           
            try
            {
                await _categoryService.Create(createCategoryDto);
                return Ok("Thêm loại tài sản thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateCategoryDto updateCategoryDto, int id)
        {
          
            try
            {
                await _categoryService.Update(updateCategoryDto,id);
                return Ok("Sửa loại tài sản thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id<=0)
            {
                return BadRequest("Id không hợp lệ");
            }
            try
            {
                await _categoryService.Delete(id);
                return Ok("Xóa loại tài sản thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<CategoryResponseDto> GetById(int id);
        public Task<List<CategoryResponseDto>> GetAll(string? search, string sortBy, bool desc);
        public Task Create(CreateCategoryDto createCategoryDto);
        public Task Update(UpdateCategoryDto updateCategoryDto, int id);
        public Task Delete(int id);
    }
}

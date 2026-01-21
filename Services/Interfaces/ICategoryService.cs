using QuanLyTaiSan.Dtos.Category;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<CategoryResponseDto> GetById(int id);
        public Task<CategoryAllDtocs> GetAll(int pageIndex, int pageSize, string? search, int? status, string sortBy, bool desc);

        public Task Create(CreateCategoryDto createCategoryDto);
        public Task Update(UpdateCategoryDto updateCategoryDto, int id);
        public Task Delete(int id);
    }
}

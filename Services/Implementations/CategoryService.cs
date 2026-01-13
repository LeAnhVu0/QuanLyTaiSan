using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task Create(CreateCategoryDto createCategoryDto)
        {
            var newCategory = new Category
            {
                CategoryName = createCategoryDto.CategoryName,
                Description = createCategoryDto.Description,
                Status = CategoryStatus.DangSuDung,
                CreatedTime = DateTime.Now
            };
            await _repo.Create(newCategory);
        }

        public async Task Delete(int id)
        {
            var category = await _repo.GetById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tồn tại loại tài sản có id = " + id);
            }
            else
            {
                bool hasAssets = await _repo.CheckAssetInCategory(id);
                if (hasAssets)
                {
                    throw new InvalidOperationException("Không thể xóa: Loại tài sản này đang được sử dụng.");
                }
                await _repo.Delete(category);
            }
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            var category = await _repo.GetAll();
            return category.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Status = c.Status.ToDisplayName(),
                CreatedTime = c.CreatedTime,
                UpdatedTime = c.UpdatedTime,
                AssetIds = c.Assets.Select(a => a.AssetId).ToList()
            }).ToList();
        }

        public async Task<CategoryDto> GetById(int id)
        {
            var category = await _repo.GetById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy loại tài sản có id = " + id);
            }
            else
            {
                return new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    Status = category.Status.ToDisplayName(),
                    CreatedTime = category.CreatedTime,
                    UpdatedTime = category.UpdatedTime,
                    AssetIds = category.Assets.Select(x => x.AssetId).ToList()
                };
            }

        }

        public async Task Update(UpdateCategoryDto updateCategoryDto, int id)
        {
            var category = await _repo.GetById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Không tìm thấy loại tài sản có id = " + id);
            }
            else
            {
                category.CategoryName = updateCategoryDto.CategoryName;
                category.Description = updateCategoryDto.Description;
                category.Status = (CategoryStatus)updateCategoryDto.Status;
                category.UpdatedTime = DateTime.Now;
                await _repo.Update();
            }
        }
    }
}

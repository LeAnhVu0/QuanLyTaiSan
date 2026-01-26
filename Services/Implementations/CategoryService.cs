using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Asset;
using QuanLyTaiSan.Dtos.Category;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Dtos.Category;
using QuanLyTaiSanTest.Enum;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IAssetRepository _assetRepo;

        public CategoryService(ICategoryRepository repo , IAssetRepository assetRepo)
        {
            _repo = repo;
            _assetRepo = assetRepo;
        }

        public async Task Create(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                throw new BadHttpRequestException("Dữ liệu gửi lên bị null");
            }
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
                throw new KeyNotFoundException("Loại tài sản không tồn tại");
            }
            else
            {
                bool hasAssets = await _repo.CheckAssetInCategory(id);
                if (hasAssets)
                {
                    throw new InvalidOperationException("Không thể xóa vì loại tài sản này đang được sử dụng");
                }
                await _repo.Delete(category);
            }
        }

        public async Task<CategoryAllDtocs> GetPageList(int pageIndex, int pageSize, string? search, int? status, string sortBy, bool desc)
        {
            var category = await _repo.GetPageList(pageIndex,pageSize, search, status, sortBy, desc);
            if (category.Items == null || category.Items.Count == 0)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }

            var items =  category.Items.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Status = c.Status.ToDisplayName(),
                CreatedTime = c.CreatedTime,
                UpdatedTime = c.UpdatedTime,
                AssetIds = c.Assets.Select(a => a.AssetId).ToList()
            }).ToList();

            var totalPage = (int)Math.Ceiling(category.TotalCount / (double)pageSize);

            return new CategoryAllDtocs
            {
                ListAsset = items,
                SearchName = search,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = category.TotalCount,
                TotalPage = totalPage,

                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPage
            };
        }
        public async Task<List<CategoryResponseDto>> GetAll()
        {
            var listCategory = await _repo.GetAll();
            if (listCategory == null)
            {
                throw new KeyNotFoundException("Không có dữ liệu");
            }
            else
            {
                return listCategory.Select(c => new CategoryResponseDto
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

        }
        public async Task<CategoryDetailDtocs> GetById(int id)
        {
            var category = await _repo.GetById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Loại tài sản không tồn tại");
            }
            else
            {
                return new CategoryDetailDtocs
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    Status = category.Status.ToDisplayName(),
                    CreatedTime = category.CreatedTime,
                    UpdatedTime = category.UpdatedTime,
                    Assets = category.Assets.Where(h => h.IsDelete == false).Select(h => new AssetNameDto
                    { 
                        AssetId = h.AssetId,
                        AssetCode = h.AssetCode,
                        AssetName = h.AssetName,
                        AssetStatus = h.Status.ToDisplayName()
                    }).ToList()
                };
            }

        }

        public async Task Update(UpdateCategoryDto updateCategoryDto, int id)
        {
            var category = await _repo.GetById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Loại tài sản không tồn tại");
            }
            bool hasAssets = await _repo.CheckAssetInCategory(id);
            
            if (category.Status == CategoryStatus.DangSuDung && updateCategoryDto.Status != 1 && hasAssets)
            {
                throw new InvalidOperationException("Không thể ngừng sử dụng loại tài sản này vì vẫn còn tài sản bên trong");
            }
            category.CategoryName = updateCategoryDto.CategoryName;
            category.Description = updateCategoryDto.Description;
            category.Status = (CategoryStatus)updateCategoryDto.Status;
            category.UpdatedTime = DateTime.Now;
            await _repo.Update();


        }
    }
}

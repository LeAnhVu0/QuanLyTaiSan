using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSanTest.Dtos.Asset;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IAssetHistoryService _assetHistoryService;

        public AssetController(IAssetService assetService,IAssetHistoryService assetHistoryService)
        {
            _assetService = assetService;
            _assetHistoryService = assetHistoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchName, int? categoryId , int? status, int pageIndex = 1, int pageSize = 3)
        {
           
            try
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                if(pageSize < 1)
                {
                    pageSize = 3;
                }
                return Ok(await _assetService.GetAll(searchName,categoryId,status,pageIndex,pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("sort")]
        public async Task<IActionResult> Sort(string sortBy = "name" , bool desc = true)
        {

            try
            {
                return Ok(await _assetService.SortAssets(sortBy,desc));
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
                return Ok(await _assetService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAssetDto createAssetDto)
        {
            try
            {
                await _assetService.Create(createAssetDto);

                return Ok("Thêm tài sản thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                await _assetService.Delete(id);
                return Ok("Xóa thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,UpdateAssetDto updateAssetDto)
        {
            if (id <= 0)
            {
                return BadRequest("Id không hợp lệ");
            }
            try
            {
                await _assetService.Update(updateAssetDto,id);
                return Ok("Sửa thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("History")]
        public async Task<IActionResult> GetAllHistory()
        {
            try
            {
                return Ok( await _assetHistoryService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

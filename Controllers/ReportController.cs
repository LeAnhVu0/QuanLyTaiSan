using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService) 
        {
            _reportService = reportService;
        }
        [HttpGet("Report-History")]
        public async Task<IActionResult> GetAllReport()
        {
            return Ok(await _reportService.GetAllReport());
        }

        [HttpGet("Category-Analytics")]
        public async Task<IActionResult> GetCategoryAnalytics()
        {
            return Ok(await _reportService.GetCategoryAnalytics());
        }
        [HttpGet("Category-Deltails")]
        public async Task<IActionResult> GetCategoryDeltails()
        {
            return Ok(await _reportService.GetCategoryDetails(null));
        }
    }
}

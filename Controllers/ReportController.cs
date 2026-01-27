using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;

namespace QuanLyTaiSanTest.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService) 
        {
            _reportService = reportService;
        }
        //[HttpGet("Report-History")]
        //public async Task<IActionResult> GetAllReport()
        //{
        //    return Ok(await _reportService.GetAllReport());
        //}

        [HttpGet("Category-Analytics")]
        public async Task<IActionResult> GetCategoryAnalytics()
        {
            return Ok(await _reportService.GetCategoryAnalytics());
        }
        [HttpGet("Department-Analytics")]
        public async Task<IActionResult> GetDepartmentAnalytics()
        {
            return Ok(await _reportService.GetDepartmentAnalytics());
        }
        [HttpGet("Status-Analytics")]
        public async Task<IActionResult> GetStatusAnalytics()
        {
            return Ok(await _reportService.GetStatusAnalytics());
        }
        [HttpGet("Fluctuation")]
        public async Task<IActionResult> GetFluctuation([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _reportService.GetFluctuation(from, to);
            return Ok(result);
        }

        //[HttpGet("Category-Deltails")]
        //public async Task<IActionResult> GetCategoryDeltails()
        //{
        //    return Ok(await _reportService.GetCategoryDetails(null));
        //}
    }
}

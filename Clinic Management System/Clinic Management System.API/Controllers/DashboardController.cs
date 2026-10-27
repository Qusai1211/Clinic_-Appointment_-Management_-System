using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("specialty-chart")]
        public async Task<IActionResult> GetSpecialtyChartData()
        {
            var data = await _dashboardService.GetSpecialtyChartDataAsync();
            return Ok(data);
        }
    }
}

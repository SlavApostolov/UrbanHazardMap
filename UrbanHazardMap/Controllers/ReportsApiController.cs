using Microsoft.AspNetCore.Mvc;
using UrbanHazardMap.Models;
using UrbanHazardMap.Services;

namespace UrbanHazardMap.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsApiController : ControllerBase
    {
        private readonly IIssueReportService _issueReportService;

        public ReportsApiController(IIssueReportService issueReportService)
        {
            _issueReportService = issueReportService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApprovedReportDto>>> Get()
        {
            var reports = await _issueReportService.GetApprovedAsync();

            var result = reports.Select(report => new ApprovedReportDto
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                CreatedAt = report.CreatedAt,
                Categories = report.Categories
                    .Select(category => new ApprovedReportCategoryDto
                    {
                        Title = category.Title,
                        Color = category.MarkedHexColor
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }
    }
}

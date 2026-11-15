using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;
using UrbanHazardMap.Services;
using UrbanHazardMap.ViewModels;

namespace UrbanHazardMap.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminReportController : Controller
    {
        private readonly IIssueReportService _issueReportService;

        public AdminReportController(IIssueReportService issueReportService)
        {
            _issueReportService = issueReportService;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _issueReportService.GetByStatusAsync(ReportStatus.Pending);

            var model = reports
                .Select(report => new AdminReportListItemViewModel
                {
                    Id = report.Id,
                    Title = report.Title,
                    CreatedAt = report.CreatedAt,
                    ReporterEmail = report.ReportedByUser?.Email ?? string.Empty,
                    Categories = report.Categories.Select(MapCategory).ToList()
                })
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _issueReportService.GetByIdAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            var model = new AdminReportDetailsViewModel
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                CreatedAt = report.CreatedAt,
                ReporterEmail = report.ReportedByUser?.Email ?? string.Empty,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                Status = report.Status.ToString(),
                Categories = report.Categories.Select(MapCategory).ToList()
            };

            return View(model);
        }

        private static AdminReportCategoryViewModel MapCategory(Category category)
        {
            return new AdminReportCategoryViewModel
            {
                Title = category.Title,
                Color = category.MarkedHexColor,
                InstitutionName = category.Institution?.Name ?? string.Empty
            };
        }
    }
}

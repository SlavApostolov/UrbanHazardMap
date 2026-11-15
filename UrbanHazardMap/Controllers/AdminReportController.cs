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
                ReviewedAt = report.ReviewedAt,
                RejectionReason = report.RejectionReason,
                Categories = report.Categories.Select(MapCategory).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var approved = await _issueReportService.ApproveAsync(id);

            SetMessage(approved, "Report approved.");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var rejected = await _issueReportService.RejectAsync(id, reason);

            SetMessage(rejected, "Report rejected.");

            return RedirectToAction(nameof(Index));
        }

        private void SetMessage(bool succeeded, string successMessage)
        {
            if (succeeded)
            {
                TempData["Message"] = successMessage;
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "This report was not found or was already reviewed.";
                TempData["MessageType"] = "warning";
            }
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

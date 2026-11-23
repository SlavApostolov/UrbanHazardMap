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

        public async Task<IActionResult> Index(string tab = "pending")
{
    tab = tab?.ToLowerInvariant() == "approved"
        ? "approved"
        : "pending";

    var pendingReports = await _issueReportService
        .GetByStatusAsync(ReportStatus.Pending);

    var approvedReports = await _issueReportService
        .GetByStatusAsync(ReportStatus.Approved);

    var rejectedReports = await _issueReportService
        .GetByStatusAsync(ReportStatus.Rejected);

    var selectedReports = tab == "approved"
        ? approvedReports
        : pendingReports;

    var model = new AdminReportsDashboardViewModel
    {
        ActiveTab = tab,
        PendingCount = pendingReports.Count(),
        ApprovedCount = approvedReports.Count(),
        RejectedCount = rejectedReports.Count(),

        Reports = selectedReports
            .OrderByDescending(r => r.CreatedAt)
            .Select(report => new AdminReportListItemViewModel
            {
                Id = report.Id,
                Title = report.Title,
                CreatedAt = report.CreatedAt,
                ReporterEmail = report.ReportedByUser?.Email ?? string.Empty,
                Categories = report.Categories.Select(MapCategory).ToList()
            })
            .ToList()
    };

    return View(model);
}
        public async Task<IActionResult> Details(int id)
{
    var report = await _issueReportService.GetByIdAsync(id);

    if (report == null)
    {
        return NotFound();
    }

    var notifications = await _issueReportService.GetNotificationsAsync(id);

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
        PhotoPath = report.PhotoPath,
        Categories = report.Categories.Select(MapCategory).ToList(),
        Notifications = notifications
    };

    return View(model);
}
        
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Approve(int id)
{
    var result = await _issueReportService.ApproveAsync(id);

    if (!result.Approved)
    {
        TempData["Message"] =
            "This report was not found or was already reviewed.";

        TempData["MessageType"] = "warning";

        return RedirectToAction(nameof(Index));
    }

    if (result.FailedNotifications > 0)
    {
        TempData["Message"] =
            $"Report approved, but {result.FailedNotifications} email(s) could not be sent.";

        TempData["MessageType"] = "warning";
    }
    else
    {
        TempData["Message"] = "Report approved successfully.";
        TempData["MessageType"] = "success";
    }

    return RedirectToAction(nameof(Details), new { id });
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

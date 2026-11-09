using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanHazardMap.Services;
using UrbanHazardMap.ViewModels;

namespace UrbanHazardMap.Controllers
{
    public class IssueReportController : Controller
    {
        private readonly IIssueReportService _issueReportService;

        public IssueReportController(IIssueReportService issueReportService)
        {
            _issueReportService = issueReportService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = new CreateIssueReportViewModel
            {
                AvailableCategories = await _issueReportService.GetAvailableCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIssueReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await _issueReportService.GetAvailableCategoriesAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _issueReportService.CreateAsync(model, userId);
            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

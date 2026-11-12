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
            var model = new CreateIssueReportViewModel();
            await LoadCategoriesAsync(model);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIssueReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _issueReportService.CreateAsync(
                model.Title,
                model.Description,
                model.Latitude!.Value,
                model.Longitude!.Value,
                model.CategoryIds,
                userId);

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            return View();
        }

        private async Task LoadCategoriesAsync(CreateIssueReportViewModel model)
        {
            var categories = await _issueReportService.GetAllCategoriesAsync();
            model.AvailableCategories = categories
                .Select(category => new CategoryOptionViewModel
                {
                    Id = category.Id,
                    Title = category.Title,
                    MarkedHexColor = category.MarkedHexColor
                })
                .ToList();
        }
    }
}

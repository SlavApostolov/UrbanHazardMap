using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanHazardMap.Services;
using UrbanHazardMap.ViewModels;
using UrbanHazardMap.Data.Models;
using UrbanHazardMap.Data;

namespace UrbanHazardMap.Controllers
{
    public class IssueReportController : Controller
    {
        private readonly IIssueReportService _issueReportService;
        private readonly IPhotoStorageService _photoStorageService;

public IssueReportController(
    IIssueReportService issueReportService,
    IPhotoStorageService photoStorageService)
{
    _issueReportService = issueReportService;
    _photoStorageService = photoStorageService;
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
[RequestSizeLimit(10_000_000)]
public async Task<IActionResult> Create(
    CreateIssueReportViewModel model)
{
    const long maxPhotoSize = 5 * 1024 * 1024;

    if (model.Photo != null)
    {
        var extension = Path.GetExtension(
            model.Photo.FileName).ToLowerInvariant();

        if (extension != ".jpg" &&
            extension != ".jpeg" &&
            extension != ".png")
        {
            ModelState.AddModelError(
                nameof(model.Photo),
                "Only JPG and PNG photos are allowed.");
        }

        if (model.Photo.Length > maxPhotoSize)
        {
            ModelState.AddModelError(
                nameof(model.Photo),
                "Photo must not exceed 5 MB.");
        }

        if (model.Photo.Length == 0)
        {
            ModelState.AddModelError(
                nameof(model.Photo),
                "The selected photo is empty.");
        }
    }

    if (!ModelState.IsValid)
    {
        await LoadCategoriesAsync(model);
        return View(model);
    }

    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    string? photoPath = null;

    if (model.Photo != null)
    {
        try
        {
            await using var stream =
                model.Photo.OpenReadStream();

            photoPath = await _photoStorageService.SaveAsync(
                stream,
                model.Photo.FileName);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                nameof(model.Photo),
                ex.Message);

            await LoadCategoriesAsync(model);
            return View(model);
        }
    }

    try
    {
        await _issueReportService.CreateAsync(
            model.Title,
            model.Description,
            model.Latitude!.Value,
            model.Longitude!.Value,
            model.CategoryIds,
            userId,
            photoPath);
    }
    catch (ArgumentException)
    {
        ModelState.AddModelError(
            nameof(model.CategoryIds),
            "One or more selected categories are invalid.");

        await LoadCategoriesAsync(model);
        return View(model);
    }

    return RedirectToAction(nameof(Success));
}

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole(RoleNames.Admin);
            var report = await _issueReportService.GetVisibleByIdAsync(id, userId, isAdmin);

            if (report == null)
            {
                return NotFound();
            }

            var model = new IssueReportDetailsViewModel
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                CreatedAt = report.CreatedAt,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                Status = report.Status.ToString(),
                PhotoPath = report.PhotoPath,
                Categories = report.Categories.Select(MapCategory).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            // The service decides if the vote is allowed. If it is blocked (own report,
            // not approved), nothing changes and the user simply returns to the page.
            await _issueReportService.ToggleConfirmationAsync(id, userId);

            return RedirectToAction(nameof(Details), new { id });
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



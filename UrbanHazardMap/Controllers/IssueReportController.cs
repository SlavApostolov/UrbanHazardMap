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

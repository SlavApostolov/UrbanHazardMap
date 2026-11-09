using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;
using UrbanHazardMap.ViewModels;

namespace UrbanHazardMap.Services
{
    public class IssueReportService : IIssueReportService
    {
        private readonly ApplicationDbContext _context;

        public IssueReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryOptionViewModel>> GetAvailableCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(category => category.Id)
                .Select(category => new CategoryOptionViewModel
                {
                    Id = category.Id,
                    Title = category.Title,
                    MarkedHexColor = category.MarkedHexColor
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateIssueReportViewModel model, string? userId)
        {
            var categories = await _context.Categories
                .Where(category => model.CategoryIds.Contains(category.Id))
                .ToListAsync();

            var report = new IssueReport
            {
                Title = model.Title,
                Description = model.Description,
                Latitude = model.Latitude!.Value,
                Longitude = model.Longitude!.Value,
                ReportedBy = userId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Categories = categories
            };

            _context.IssueReports.Add(report);
            await _context.SaveChangesAsync();
        }
    }
}

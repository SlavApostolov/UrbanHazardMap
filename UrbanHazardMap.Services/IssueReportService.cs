using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public class IssueReportService : IIssueReportService
    {
        private readonly ApplicationDbContext _context;

        public IssueReportService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(category => category.Title)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId)
        {
            var categories = await _context.Categories
                .Where(category => categoryIds.Contains(category.Id))
                .ToListAsync();

            if (categories.Count == 0)
            {
                throw new ArgumentException("There are no Ids coresponding to the given categoryIds");
            }

            var issueReport = new IssueReport
            {
                Title = title,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                Categories = categories,
                ReportedById = userId,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.IssueReports.Add(issueReport);
            await _context.SaveChangesAsync();
            return issueReport.Id;
        }
    }
}

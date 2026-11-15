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

        public async Task<List<IssueReport>> GetApprovedAsync()
        {
            return await _context.IssueReports
                .Include(report => report.Categories)
                .Where(report => report.Status == ReportStatus.Approved)
                .OrderByDescending(report => report.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<IssueReport>> GetByStatusAsync(ReportStatus status)
        {
            return await _context.IssueReports
                .Include(report => report.Categories)
                .Include(report => report.ReportedByUser)
                .Where(report => report.Status == status)
                .OrderByDescending(report => report.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IssueReport?> GetByIdAsync(int id)
        {
            return await _context.IssueReports
                .Include(report => report.Categories)
                .ThenInclude(Category => Category.Institution)
                .Include(report => report.ReportedByUser)
                .FirstOrDefaultAsync(report => report.Id == id);
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

        public async Task<bool> ApproveAsync(int id)
        {
            var updatedRows = await _context.IssueReports
                .Where(report => report.Id == id && report.Status == ReportStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(report => report.Status, ReportStatus.Approved)
                    .SetProperty(report => report.ReviewedAt, (DateTime?)DateTime.UtcNow));

            return updatedRows == 1;
        }

        public async Task<bool> RejectAsync(int id, string? reason)
        {
            var trimmedReason = reason?.Trim();

            if (string.IsNullOrEmpty(trimmedReason))
            {
                trimmedReason = null;
            }
            else if (trimmedReason.Length > 500)
            {
                trimmedReason = trimmedReason.Substring(0, 500);
            }

            var updatedRows = await _context.IssueReports
                .Where(report => report.Id == id && report.Status == ReportStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(report => report.Status, ReportStatus.Rejected)
                    .SetProperty(report => report.RejectionReason, trimmedReason)
                    .SetProperty(report => report.ReviewedAt, (DateTime?)DateTime.UtcNow));

            return updatedRows == 1;
        }
    }
}

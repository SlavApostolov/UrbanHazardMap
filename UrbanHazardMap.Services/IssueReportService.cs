using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public class IssueReportService : IIssueReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportNotificationService _notificationService;
private readonly ILogger<IssueReportService> _logger;

        public IssueReportService(
    ApplicationDbContext applicationDbContext,
    IReportNotificationService notificationService,
    ILogger<IssueReportService> logger)
{
    _context = applicationDbContext;
    _notificationService = notificationService;
    _logger = logger;
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
                .Include(report => report.Confirmations)
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
                .AsNoTracking()
                .FirstOrDefaultAsync(report => report.Id == id);
        }
        public async Task<IssueReport?> GetVisibleByIdAsync(int id, string? userId, bool isAdmin)
        {
            
            var report =   await _context.IssueReports
                .Include(report => report.Categories)
                .ThenInclude(Category => Category.Institution)
                .AsNoTracking()
                .FirstOrDefaultAsync(report => report.Id == id);

            if (report == null)
            {
                return null;
            }
            else if (report.Status == ReportStatus.Approved || report.Status == ReportStatus.Resolved)
            {
                return report;
            }
            else if (isAdmin)
            {
                return report;
            }
            else if (userId != null && userId == report.ReportedById)
            {
                return report;
            }

            return null;
        }

        public async Task<int> GetConfirmationCountAsync(int reportId)
        {
            return await _context.ReportConfirmations
                .Where(c => c.IssueReportId == reportId)
                .CountAsync();
        }

        public async Task<bool> HasUserConfirmedAsync(int reportId, string userId)
        {
            return await _context.ReportConfirmations
                .AnyAsync(c => c.IssueReportId == reportId && c.UserId == userId);
        }

        public async Task<bool> ToggleConfirmationAsync(int reportId, string userId)
        {
            var report = await _context.IssueReports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reportId);

            // Only existing, approved reports can be confirmed, and never by their own author.
            if (report == null ||
                report.Status != ReportStatus.Approved ||
                report.ReportedById == userId)
            {
                return false;
            }

            var existingConfirmation = await _context.ReportConfirmations
                .FirstOrDefaultAsync(c => c.IssueReportId == reportId && c.UserId == userId);

            try
            {
                if (existingConfirmation != null)
                {
                    _context.ReportConfirmations.Remove(existingConfirmation);
                }
                else
                {
                    _context.ReportConfirmations.Add(new ReportConfirmation
                    {
                        IssueReportId = reportId,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // A double click: the unique index (or a vanished row) means the
                // other request already did the work, so this is not an error.
            }

            return true;
        }

        public async Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId,string? photoPath = null)
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
                PhotoPath = photoPath,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.IssueReports.Add(issueReport);
            await _context.SaveChangesAsync();
            return issueReport.Id;
        }

        
public async Task<ApprovalResult> ApproveAsync(int id)
{
    var updatedRows = await _context.IssueReports
        .Where(report =>
            report.Id == id &&
            report.Status == ReportStatus.Pending)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(
                report => report.Status,
                ReportStatus.Approved)
            .SetProperty(
                report => report.ReviewedAt,
                (DateTime?)DateTime.UtcNow));

    if (updatedRows != 1)
    {
        return new ApprovalResult
        {
            Approved = false,
            FailedNotifications = 0
        };
    }

    // At this point the report is already approved in PostgreSQL.
    try
    {
        var notifications = await _notificationService
            .NotifyInstitutionsAsync(id);

        return new ApprovalResult
        {
            Approved = true,
            FailedNotifications = notifications.Count(
                n => !n.IsSuccessful)
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Notification processing failed for approved report {ReportId}",
            id);

        // Never undo the saved approval.
        return new ApprovalResult
        {
            Approved = true,
            FailedNotifications = 1
        };
    }
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

        public async Task<List<ReportNotification>> GetNotificationsAsync(int reportId)
{
    return await _context.ReportNotifications
        .Include(n => n.Institution)
        .Where(n => n.IssueReportId == reportId)
        .OrderByDescending(n => n.SentAt)
        .AsNoTracking()
        .ToListAsync();
}
    }

    
}

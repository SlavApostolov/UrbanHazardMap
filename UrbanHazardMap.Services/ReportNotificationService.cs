
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrbanHazardMap.Data;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public class ReportNotificationService : IReportNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IPhotoStorageService _photoStorageService;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<ReportNotificationService> _logger;

        public ReportNotificationService(
            ApplicationDbContext context,
            IEmailService emailService,
            IPhotoStorageService photoStorageService,
            IOptions<EmailSettings> emailSettings,
            ILogger<ReportNotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _photoStorageService = photoStorageService;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<List<ReportNotification>> NotifyInstitutionsAsync(
            int reportId)
        {
            var report = await _context.IssueReports
                .Include(r => r.Categories)
                    .ThenInclude(c => c.Institution)
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.Id == reportId &&
                    r.Status == ReportStatus.Approved);

            if (report == null)
            {
                throw new InvalidOperationException(
                    $"Approved report {reportId} was not found.");
            }

            // Prepare all groups before starting concurrent tasks.
            var groups = report.Categories
                .Where(c => c.Institution != null)
                .GroupBy(c => c.InstitutionId)
                .Select(group => new
                {
                    Institution = group.First().Institution!,
                    Categories = group.ToList()
                })
                .ToList();

            // Each task sends an email independently.
            // None of these tasks accesses the DbContext.
            var tasks = groups.Select(group =>
                SendToInstitutionAsync(
                    report,
                    group.Institution,
                    group.Categories));

            var notifications = (await Task.WhenAll(tasks)).ToList();

            // Only use DbContext after all concurrent sends finish.
            _context.ReportNotifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            return notifications;
        }

        private async Task<ReportNotification> SendToInstitutionAsync(
            IssueReport report,
            Institution institution,
            List<Category> categories)
        {
            var notification = new ReportNotification
            {
                IssueReportId = report.Id,
                InstitutionId = institution.Id,
                SentAt = DateTime.UtcNow,
                IsSuccessful = false
            };

            try
            {
                var recipient = string.IsNullOrWhiteSpace(
                    _emailSettings.OverrideRecipient)
                    ? institution.ContactEmail
                    : _emailSettings.OverrideRecipient;

                var htmlBody = ReportNotificationEmailBuilder.BuildBody(
                    report, categories);

                var subject = $"New approved report: {report.Title}";

                // Open a separate stream for each email.
                Stream? photoStream = null;

                try
                {
                    EmailAttachment? attachment = null;

                    if (!string.IsNullOrWhiteSpace(report.PhotoPath))
                    {
                        try
                        {
                            photoStream = await _photoStorageService
                                .OpenReadAsync(report.PhotoPath);

                            if (photoStream != null)
                            {
                                var extension = Path.GetExtension(
                                    report.PhotoPath).ToLowerInvariant();

                                var contentType = extension == ".png"
                                    ? "image/png"
                                    : "image/jpeg";

                                attachment = new EmailAttachment
                                {
                                    FileName = extension == ".png"
                                        ? "report-photo.png"
                                        : "report-photo.jpg",
                                    ContentType = contentType,
                                    Content = photoStream
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(
                                ex,
                                "Could not load photo for report {ReportId}",
                                report.Id);

                            // Photo failure must not prevent email delivery.
                            photoStream?.Dispose();
                            photoStream = null;
                        }
                    }

                    await _emailService.SendAsync(
                        recipient,
                        institution.Name,
                        subject,
                        htmlBody,
                        attachment);
                }
                finally
                {
                    photoStream?.Dispose();
                }

                notification.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                notification.Error = ex.Message.Length > 500
                    ? ex.Message[..500]
                    : ex.Message;

                _logger.LogError(
                    ex,
                    "Failed to notify institution {InstitutionId} for report {ReportId}",
                    institution.Id,
                    report.Id);
            }

            notification.SentAt = DateTime.UtcNow;
            return notification;
        }
    }
}

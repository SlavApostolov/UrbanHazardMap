using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<Category>> GetAllCategoriesAsync();

        Task<List<IssueReport>> GetApprovedAsync();

        Task<List<IssueReport>> GetByStatusAsync(ReportStatus status);

        Task<IssueReport?> GetByIdAsync(int id);

        Task<IssueReport?> GetVisibleByIdAsync(int id, string? userId, bool isAdmin);

        Task<int> GetConfirmationCountAsync(int reportId);

        Task<bool> ToggleConfirmationAsync(int reportId, string userId);

        Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId,string? photoPath = null);

        Task<bool> ApproveAsync(int id);

        Task<bool> RejectAsync(int id, string? reason);
    }
}

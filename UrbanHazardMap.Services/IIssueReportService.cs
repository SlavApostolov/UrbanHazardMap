using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<Category>> GetAllCategoriesAsync();

        Task<List<IssueReport>> GetApprovedAsync();

        Task<List<IssueReport>> GetByStatusAsync(ReportStatus status);

        Task<IssueReport?> GetByIdAsync(int id);

        Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId);
    }
}

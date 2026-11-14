using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<Category>> GetAllCategoriesAsync();

        Task<List<IssueReport>> GetApprovedAsync();

        Task<int> CreateAsync(string title, string description, double latitude, double longitude, IEnumerable<int> categoryIds, string? userId);
    }
}

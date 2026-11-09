using UrbanHazardMap.ViewModels;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<CategoryOptionViewModel>> GetAvailableCategoriesAsync();

        Task CreateAsync(CreateIssueReportViewModel model, string? userId);
    }
}

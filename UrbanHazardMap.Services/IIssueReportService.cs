using UrbanHazardMap.Data.Models;
using UrbanHazardMap.ViewModels;

namespace UrbanHazardMap.Services
{
    public interface IIssueReportService
    {
        Task<List<CategoryOptionViewModel>> GetAvailableCategoriesAsync();

        Task<List<IssueReport>> GetApprovedAsync();

        Task CreateAsync(CreateIssueReportViewModel model, string? userId);
    }
}


using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public interface IReportNotificationService
    {
        Task<List<ReportNotification>> NotifyInstitutionsAsync(int reportId);
    }
}

namespace UrbanHazardMap.ViewModels
{
    public class AdminReportListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ReporterEmail { get; set; } = string.Empty;
        public List<AdminReportCategoryViewModel> Categories { get; set; } = new List<AdminReportCategoryViewModel>();
    }
}

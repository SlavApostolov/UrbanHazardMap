namespace UrbanHazardMap.ViewModels
{
    public class AdminReportDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ReporterEmail { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<AdminReportCategoryViewModel> Categories { get; set; } = new List<AdminReportCategoryViewModel>();
    }
}

namespace UrbanHazardMap.ViewModels
{
    public class IssueReportDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public int ConfirmationCount { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool CanConfirm { get; set; }
        public bool HasConfirmed { get; set; }
        public List<AdminReportCategoryViewModel> Categories { get; set; } = new List<AdminReportCategoryViewModel>();
    }
}

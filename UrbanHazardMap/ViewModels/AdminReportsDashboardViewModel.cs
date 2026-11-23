namespace UrbanHazardMap.ViewModels
{
    public class AdminReportsDashboardViewModel
    {
        public string ActiveTab { get; set; } = "pending";

        public int PendingCount { get; set; }

        public int ApprovedCount { get; set; }

        public int RejectedCount { get; set; }

        public List<AdminReportListItemViewModel> Reports { get; set; }
            = new();
    }
}
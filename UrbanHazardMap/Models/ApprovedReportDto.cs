namespace UrbanHazardMap.Models
{
    public class ApprovedReportDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ApprovedReportCategoryDto> Categories { get; set; } = new();
    }

    public class ApprovedReportCategoryDto
    {
        public string Title { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;
    }
}

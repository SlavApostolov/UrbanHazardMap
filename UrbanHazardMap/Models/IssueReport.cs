using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.Models
{
    public class IssueReport
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ReportedBy { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Category> Categories { get; set; }
    = new List<Category>();
    }
}

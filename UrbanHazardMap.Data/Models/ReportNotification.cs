
using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.Data.Models
{
    public class ReportNotification
    {
        public int Id { get; set; }

        [Required]
        public int IssueReportId { get; set; }

        public IssueReport IssueReport { get; set; } = null!;

        [Required]
        public int InstitutionId { get; set; }

        public Institution Institution { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public bool IsSuccessful { get; set; }

        [MaxLength(500)]
        public string? Error { get; set; }
    }
}

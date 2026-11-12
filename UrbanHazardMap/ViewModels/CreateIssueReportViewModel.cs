using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.ViewModels
{
    public class CreateIssueReportViewModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double? Longitude { get; set; }

        [MinLength(1, ErrorMessage = "Select at least one category.")]
        public List<int> CategoryIds { get; set; } = new();

        public List<CategoryOptionViewModel> AvailableCategories { get; set; } = new();
    }
}

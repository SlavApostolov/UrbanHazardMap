using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required, MaxLength(7)] //Cause it is #123456 = so 7 characters
        public string MarkedHexColor { get; set; }
    }
}

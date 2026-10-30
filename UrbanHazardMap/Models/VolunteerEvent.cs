using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.Models
{
    public class VolunteerEvent
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required] 
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }
        public int? MaxParticipants { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

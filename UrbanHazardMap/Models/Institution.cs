using System.ComponentModel.DataAnnotations;

namespace UrbanHazardMap.Models
{
    public class Institution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Abbreviation { get; set; }


        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        
 public Category? Category { get; set; } = null!;

    }
}

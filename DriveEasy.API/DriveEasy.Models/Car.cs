using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Make { get; set; }

        [Required]
        [StringLength(50)]
        public string? Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string? Type { get; set; }

        [Required]
        public decimal Mileage { get; set; }

        [Required]
        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        public bool Availability { get; set; }
    }
}

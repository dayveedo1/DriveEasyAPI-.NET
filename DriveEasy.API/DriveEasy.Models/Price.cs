using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Models
{
    public class Price
    {
        [Key]
        public int PriceId { get; set; }

        [Required]
        [StringLength(50)]
        public string? CarType { get; set; }

        [Required]
        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        public decimal RentalPrice { get; set; }

    }
}

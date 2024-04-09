using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("Car")]
        public int CarId { get; set; }

        [Required]
        [StringLength(100)]
        public string? PickupLocation { get; set; }

        [Required]
        [StringLength(100)]
        public string? DropoffLocation { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DropoffDate { get; set; }

        [Required]
        public TimeSpan PickupTime { get; set; }

        [Required]
        public TimeSpan DropoffTime { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(20)]
        public string? BookingStatus { get; set; }
    }
}

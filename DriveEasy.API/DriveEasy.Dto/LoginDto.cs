using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage ="Password must be 6-15 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}

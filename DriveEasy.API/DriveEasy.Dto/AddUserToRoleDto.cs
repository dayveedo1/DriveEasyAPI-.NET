using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Dto
{
    public class AddUserToRoleDto
    {

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? RoleName { get; set; }

    }
}

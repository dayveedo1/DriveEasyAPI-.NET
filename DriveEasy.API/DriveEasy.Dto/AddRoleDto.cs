using System.ComponentModel.DataAnnotations;

namespace DriveEasy.API.DriveEasy.Dto
{
    public class AddRoleDto
    {
        [Required]
        public string? RoleName { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace DriveEasy.API.DriveEasy.Models
{
    public class User : IdentityUser
    {
        
        public required string? FirstName { get; set; }
        public required string? LastName { get; set; }
        public required string? Sex { get; set; }
        public required string? DateOfBirth { get; set; }
        public required string? Address { get; set; }
        public required string? City { get; set; }
        public required string? State { get; set; }
        public required string? ZipCode { get; set; }
        public required string? Country { get; set; }
        public string? DriverLicence { get; set; }
        public string? ProfilePicture { get; set; }
         
    }
}

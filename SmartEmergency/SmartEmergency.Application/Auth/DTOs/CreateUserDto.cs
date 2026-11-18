using System.ComponentModel.DataAnnotations;

namespace SmartEmergency.Application.Auth.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
       
        [Required]
        public string Role { get; set; } = string.Empty;

        public int? HospitalId { get; set; }

        public int? DepartmentId { get; set; }

        public string? Specialization { get; set; }
    }
}

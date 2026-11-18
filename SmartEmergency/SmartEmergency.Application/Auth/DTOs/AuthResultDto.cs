namespace SmartEmergency.Application.Auth.DTOs
{
     public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
        public UserDto? User { get; set; }
    }
}

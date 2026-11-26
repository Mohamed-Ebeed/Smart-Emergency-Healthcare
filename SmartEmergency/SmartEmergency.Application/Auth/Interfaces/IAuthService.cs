using SmartEmergency.Application.Auth.DTOs;

namespace SmartEmergency.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);

        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> CheckLoginAsync(LoginDto dto);

        Task LogoutAsync();

        Task<AuthResultDto> CreateUserAsync(CreateUserDto dto);
    }
}
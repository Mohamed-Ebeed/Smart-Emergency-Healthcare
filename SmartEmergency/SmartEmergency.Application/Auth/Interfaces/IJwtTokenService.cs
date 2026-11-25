using SmartEmergency.Application.Auth.DTOs;

namespace SmartEmergency.Application.Auth.Interfaces
{
    public interface IJwtTokenService
    {
        AuthResponseDto CreateToken(UserDto user);
    }
}
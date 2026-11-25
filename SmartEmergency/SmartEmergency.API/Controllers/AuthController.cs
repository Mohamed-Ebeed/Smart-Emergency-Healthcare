using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Auth.DTOs;
using SmartEmergency.Application.Auth.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        // Patient registers himself (always gets Patient role)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(_jwtTokenService.CreateToken(result.User!));
        }

        // Login and get a token
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.CheckLoginAsync(dto);
            if (!result.Succeeded)
                return Unauthorized(new { errors = result.Errors });

            return Ok(_jwtTokenService.CreateToken(result.User!));
        }

        // JWT is stateless, the client just deletes the token
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out. Delete the token on the client." });
        }

        // Admin creates staff accounts
        [HttpPost("users")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _authService.CreateUserAsync(dto);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.User);
        }

        // Info of the logged in user (from the token)
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                fullName = User.FindFirstValue(ClaimTypes.Name),
                email = User.FindFirstValue(ClaimTypes.Email),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
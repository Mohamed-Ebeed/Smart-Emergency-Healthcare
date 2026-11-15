using Microsoft.AspNetCore.Identity;
using SmartEmergency.Application.Auth.DTOs;
using SmartEmergency.Application.Auth.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Data;

namespace SmartEmergency.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Admin creates staff accounts
        public async Task<AuthResultDto> CreateUserAsync(CreateUserDto dto)
        {
            var result = new AuthResultDto();

            // Admin can only create these roles
            var allowedRoles = new[] { Roles.Operator, Roles.Paramedic, Roles.HospitalStaff, Roles.Doctor };
            if (!allowedRoles.Contains(dto.Role))
            {
                result.Errors.Add("Invalid role.");
                return result;
            }

            // Paramedic and HospitalStaff must belong to a hospital
            if ((dto.Role == Roles.Paramedic || dto.Role == Roles.HospitalStaff) && dto.HospitalId == null)
            {
                result.Errors.Add("HospitalId is required for this role.");
                return result;
            }

            // Doctor must belong to a department
            if (dto.Role == Roles.Doctor && dto.DepartmentId == null)
            {
                result.Errors.Add("DepartmentId is required for doctors.");
                return result;
            }

            // Email must be unique
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                result.Errors.Add("Email is already registered.");
                return result;
            }

            // User + role + profile are saved together or not at all
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create identity user
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber
                };

                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    result.Errors.AddRange(createResult.Errors.Select(e => e.Description));
                    return result;
                }

                // Assign role
                var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!roleResult.Succeeded)
                {
                    result.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return result;
                }

                // Create profile based on role (Operator has no profile table)
                if (dto.Role == Roles.Doctor)
                {
                    _context.Doctors.Add(new Doctor
                    {
                        UserId = user.Id,
                        DepartmentId = dto.DepartmentId!.Value,
                        FullName = dto.FullName,
                        Specialization = dto.Specialization,
                        PhoneNumber = dto.PhoneNumber
                    });
                }
                else if (dto.Role == Roles.Paramedic)
                {
                    _context.Paramedics.Add(new Paramedic
                    {
                        UserId = user.Id,
                        HospitalId = dto.HospitalId!.Value,
                        FullName = dto.FullName,
                        PhoneNumber = dto.PhoneNumber
                    });
                }
                else if (dto.Role == Roles.HospitalStaff)
                {
                    _context.HospitalStaffs.Add(new HospitalStaff
                    {
                        UserId = user.Id,
                        HospitalId = dto.HospitalId!.Value,
                        FullName = dto.FullName,
                        PhoneNumber = dto.PhoneNumber
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return created user
                result.Succeeded = true;
                result.User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = dto.Role
                };
                return result;
            }
            catch (Exception)
            {
                // Undo everything if any step fails
                await transaction.RollbackAsync();
                result.Errors.Add("Something went wrong while creating the account.");
                return result;
            }
        }

        // Login
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var result = new AuthResultDto();

            // Same message for wrong email or wrong password (security)
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                result.Errors.Add("Invalid email or password.");
                return result;
            }

            // Each user has one role only
            var roles = await _userManager.GetRolesAsync(user);

            result.Succeeded = true;
            result.User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };
            return result;
        }

        // Patient self registration
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var result = new AuthResultDto();

            // Email must be unique
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                result.Errors.Add("Email is already registered.");
                return result;
            }

            // User + role + patient profile are saved together or not at all
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create identity user
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber
                };

                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    result.Errors.AddRange(createResult.Errors.Select(e => e.Description));
                    return result;
                }

                // Assign Patient role
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.Patient);
                if (!roleResult.Succeeded)
                {
                    result.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return result;
                }

                // Create patient profile
                var patient = new Patient
                {
                    UserId = user.Id,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    Address = dto.Address
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return registered user
                result.Succeeded = true;
                result.User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = Roles.Patient
                };
                return result;
            }
            catch (Exception)
            {
                // Undo everything if any step fails
                await transaction.RollbackAsync();
                result.Errors.Add("Something went wrong while creating the account.");
                return result;
            }
        }
    }
}
using SmartEmergency.Application.Hospitals.DTOs;

namespace SmartEmergency.Application.Hospitals.Interfaces;

// Read and update only. Accounts are created in AuthService.
public interface IStaffProfileService
{
    Task<DoctorProfileDto?> GetDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<List<DoctorProfileDto>> GetDoctorsByHospitalAsync(int hospitalId, CancellationToken ct = default);
    Task<DoctorProfileDto?> UpdateDoctorAsync(int doctorId, UpdateDoctorProfileDto dto, CancellationToken ct = default);

    Task<ParamedicProfileDto?> GetParamedicAsync(int paramedicId, CancellationToken ct = default);
    Task<List<ParamedicProfileDto>> GetParamedicsByHospitalAsync(int hospitalId, CancellationToken ct = default);
    Task<ParamedicProfileDto?> UpdateParamedicAsync(int paramedicId, UpdateParamedicProfileDto dto, CancellationToken ct = default);

    Task<HospitalStaffProfileDto?> GetHospitalStaffAsync(int hospitalStaffId, CancellationToken ct = default);
    Task<List<HospitalStaffProfileDto>> GetHospitalStaffByHospitalAsync(int hospitalId, CancellationToken ct = default);
    Task<HospitalStaffProfileDto?> UpdateHospitalStaffAsync(int hospitalStaffId, UpdateHospitalStaffProfileDto dto, CancellationToken ct = default);
}
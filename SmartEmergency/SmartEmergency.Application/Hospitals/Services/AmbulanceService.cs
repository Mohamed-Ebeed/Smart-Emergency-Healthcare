using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Services;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Hospitals.Services;

public class AmbulanceService : IAmbulanceService
{
    private readonly IAppDbContext _db;

    public AmbulanceService(IAppDbContext db) => _db = db;

    public async Task<AmbulanceDto> CreateAsync(CreateAmbulanceDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var hospitalExists = await _db.Hospitals.AnyAsync(h => h.HospitalId == dto.HospitalId, ct);
        if (!hospitalExists)
            throw new InvalidOperationException("Hospital not found.");

        // Vehicle number must be unique
        var vehicleNumber = dto.VehicleNumber.Trim();
        var numberUsed = await _db.Ambulances.AnyAsync(a => a.VehicleNumber == vehicleNumber, ct);
        if (numberUsed)
            throw new InvalidOperationException("Vehicle number is already used.");

        var ambulance = new Ambulance
        {
            HospitalId = dto.HospitalId,
            VehicleNumber = vehicleNumber,
            AmbulanceStatus = AmbulanceStatus.Available
        };

        _db.Ambulances.Add(ambulance);
        await _db.SaveChangesAsync(ct);

        return ToDto(ambulance);
    }

    public async Task<AmbulanceDto?> UpdateAsync(int ambulanceId, UpdateAmbulanceDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var ambulance = await _db.Ambulances.FirstOrDefaultAsync(a => a.AmbulanceId == ambulanceId, ct);
        if (ambulance is null) return null;

        var vehicleNumber = dto.VehicleNumber.Trim();
        var numberUsed = await _db.Ambulances.AnyAsync(a => a.VehicleNumber == vehicleNumber && a.AmbulanceId != ambulanceId, ct);
        if (numberUsed)
            throw new InvalidOperationException("Vehicle number is already used.");

        ambulance.VehicleNumber = vehicleNumber;

        await _db.SaveChangesAsync(ct);
        return ToDto(ambulance);
    }

    public Task<AmbulanceDto?> GetByIdAsync(int ambulanceId, CancellationToken ct = default) =>
        _db.Ambulances.AsNoTracking()
            .Where(a => a.AmbulanceId == ambulanceId)
            .Select(a => new AmbulanceDto
            {
                AmbulanceId = a.AmbulanceId,
                HospitalId = a.HospitalId,
                VehicleNumber = a.VehicleNumber,
                AmbulanceStatus = a.AmbulanceStatus,
                LastKnownLatitude = a.LastKnownLatitude,
                LastKnownLongitude = a.LastKnownLongitude
            })
            .FirstOrDefaultAsync(ct);

    public Task<List<AmbulanceDto>> GetByHospitalAsync(int hospitalId, CancellationToken ct = default) =>
        _db.Ambulances.AsNoTracking()
            .Where(a => a.HospitalId == hospitalId)
            .OrderBy(a => a.VehicleNumber)
            .Select(a => new AmbulanceDto
            {
                AmbulanceId = a.AmbulanceId,
                HospitalId = a.HospitalId,
                VehicleNumber = a.VehicleNumber,
                AmbulanceStatus = a.AmbulanceStatus,
                LastKnownLatitude = a.LastKnownLatitude,
                LastKnownLongitude = a.LastKnownLongitude
            })
            .ToListAsync(ct);

    // Ambulance has no IsActive, so deactivate = Offline and activate = Available
    public async Task<bool> SetActiveAsync(int ambulanceId, bool isActive, CancellationToken ct = default)
    {
        var ambulance = await _db.Ambulances.FirstOrDefaultAsync(a => a.AmbulanceId == ambulanceId, ct);
        if (ambulance is null) return false;

        if (!isActive && ambulance.AmbulanceStatus == AmbulanceStatus.Busy)
            throw new InvalidOperationException("Ambulance is on a job now and can't be deactivated.");

        ambulance.AmbulanceStatus = isActive ? AmbulanceStatus.Available : AmbulanceStatus.Offline;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(int ambulanceId, AmbulanceStatus status, CancellationToken ct = default)
    {
        if (!Enum.IsDefined(status))
            throw new InvalidOperationException("Invalid ambulance status.");

        var ambulance = await _db.Ambulances.FirstOrDefaultAsync(a => a.AmbulanceId == ambulanceId, ct);
        if (ambulance is null) return false;

        ambulance.AmbulanceStatus = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateLocationAsync(int ambulanceId, UpdateAmbulanceLocationDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var ambulance = await _db.Ambulances.FirstOrDefaultAsync(a => a.AmbulanceId == ambulanceId, ct);
        if (ambulance is null) return false;

        ambulance.LastKnownLatitude = dto.Latitude;
        ambulance.LastKnownLongitude = dto.Longitude;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static AmbulanceDto ToDto(Ambulance a) => new()
    {
        AmbulanceId = a.AmbulanceId,
        HospitalId = a.HospitalId,
        VehicleNumber = a.VehicleNumber,
        AmbulanceStatus = a.AmbulanceStatus,
        LastKnownLatitude = a.LastKnownLatitude,
        LastKnownLongitude = a.LastKnownLongitude
    };
}
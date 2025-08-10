using HospitalManagement.Core.DTOs;
using HospitalManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;


public interface ICommonService
{
    Task<List<DropdownItemDto>> GetSpecialtiesAsync(CancellationToken ct = default);
    Task<List<DropdownItemDto>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<List<DropdownItemDto>> GetPatientsAsync(CancellationToken ct = default);
    Task<List<DropdownItemDto>> GetDoctorsAsync(CancellationToken ct = default);
}

public class CommonService : ICommonService
{
    private readonly HospitalDbContext _db;
    public CommonService(HospitalDbContext db) => _db = db;

    public async Task<List<DropdownItemDto>> GetSpecialtiesAsync(CancellationToken ct = default)
    {
        return await _db.Specialties
            .AsNoTracking()
            .Select(s => new DropdownItemDto
            {
                Id = s.SpecialtyId,
                Name = s.SpecialtyName
            })
            .OrderBy(s => s.Name)
            .ToListAsync(ct);
    }

    public async Task<List<DropdownItemDto>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        return await _db.Departments
            .AsNoTracking()
            .Select(d => new DropdownItemDto
            {
                Id = d.DepartmentId,
                Name = d.DepartmentName
            })
            .OrderBy(d => d.Name)
            .ToListAsync(ct);
    }

    public async Task<List<DropdownItemDto>> GetPatientsAsync(CancellationToken ct = default)
    {
        return await _db.Patients
            .AsNoTracking()
            .Select(p => new DropdownItemDto
            {
                Id = p.PatientId,
                Name = p.FullName
            })
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<List<DropdownItemDto>> GetDoctorsAsync(CancellationToken ct = default)
    {
        return await _db.Doctors
            .AsNoTracking()
            .Select(d => new DropdownItemDto
            {
                Id = d.DoctorId,
                Name = d.FullName
            })
            .OrderBy(d => d.Name)
            .ToListAsync(ct);
    }
}

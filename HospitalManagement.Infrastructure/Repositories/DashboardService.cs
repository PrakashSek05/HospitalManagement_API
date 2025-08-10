using HospitalManagement.Core.DTOs;
using HospitalManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}

public class DashboardService : IDashboardService
{
    private readonly HospitalDbContext _db;
    public DashboardService(HospitalDbContext db) => _db = db;

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        try
        {
            var activeAppointments = await _db.Appointments
               .CountAsync(a => a.Status == "Scheduled", ct);

            var activePatients = await _db.Patients
                .CountAsync(p => p.Status.Equals(true), ct);

            var totalPatients = await _db.Patients
                .CountAsync(ct);

            var pendingBills = await _db.Billings
                .CountAsync(b => b.PaidFlag.Equals(false), ct);

            var completedBills = await _db.Billings
                .CountAsync(b => b.PaidFlag.Equals(true), ct);

            var availableDoctors = await _db.Doctors
                .CountAsync(d => d.IsAvailable == true, ct);

            var totalDoctors = await _db.Doctors
                .CountAsync(ct);

            return new DashboardSummaryDto
            {
                ActiveAppointments = activeAppointments,
                ActivePatients = activePatients,
                TotalPatients = totalPatients,
                PendingBills = pendingBills,
                CompletedBills = completedBills,
                AvailableDoctors = availableDoctors,
                TotalDoctors = totalDoctors
            };
        }
        catch (Exception ex)
        {
            return new DashboardSummaryDto
            {
                ActiveAppointments = 0,
                ActivePatients =0,
                TotalPatients = 0,
                PendingBills = 0,
                CompletedBills = 0,
                AvailableDoctors = 0,
                TotalDoctors = 0
            };
            throw;
        }
    }
}

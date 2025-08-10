using HospitalManagement.API.Dtos;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly HospitalDbContext _db;

    public PatientsController(IUnitOfWork uow, HospitalDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    // LIST (master grid)
    // Returns PatientMasterDto with last appointment & last billing dates (for popup links)
    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    var data = await _db.Patients
    //        .AsNoTracking()
    //        .Select(p => new
    //        {
    //            p.Mrn,
    //            p.Phone,
    //            p.PatientId,
    //            p.FullName,
    //            p.Status,
    //            p.CreatedUtc,
    //            p.UpdatedUtc,
    //            LatestAppt = _db.Appointments
    //                            .Where(a => a.PatientId == p.PatientId)
    //                            .OrderByDescending(a => a.VisitDateTime)
    //                            .Select(a => new { a.VisitDateTime, a.Reason })
    //                            .FirstOrDefault(),
    //            LastBillingDate = _db.Billings
    //                            .Where(b => b.PatientId == p.PatientId)
    //                            .Max(b => (DateTime?)b.CreatedUtc)
    //        })
    //        .Select(x => new PatientMasterDto
    //        {
    //            PatientId = x.PatientId,
    //            Mrn = x.Mrn,
    //            Phone = x.Phone,
    //            FullName = x.FullName,
    //            Status = x.Status,
    //            CreatedUtc = x.CreatedUtc,
    //            UpdatedUtc = x.UpdatedUtc,
    //            LatestVisitTime = x.LatestAppt != null ? x.LatestAppt.VisitDateTime : null,
    //            LatestReason = x.LatestAppt != null ? x.LatestAppt.Reason : null,
    //            Last_AppointmentDate = x.LatestAppt.VisitDateTime,
    //            Last_BillingDate = x.LastBillingDate ?? default
    //        })
    //        .ToListAsync();

    //    return Ok(data);
    //}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _db.Patients
            .AsNoTracking()
            .Select(p => new
            {
                p.PatientId,
                p.Mrn,
                p.Phone,
                p.FullName,
                p.Status,
                p.CreatedUtc,   // if these are varchar in DB, also remove them here temporarily
                p.UpdatedUtc
            })
            .ToListAsync();

        var ids = patients.Select(p => p.PatientId).ToList();

        // Fetch appointments (raw)
        var apptsRaw = await _db.Appointments
            .AsNoTracking()
            .Where(a => ids.Contains(a.PatientId))
            .Select(a => new { a.PatientId, a.VisitDateTime, a.Reason }) // if VisitDateTime is varchar in DB, EF property type should be string
            .ToListAsync();

        // Fetch billings (raw)
        var billsRaw = await _db.Billings
            .AsNoTracking()
            .Where(b => ids.Contains(b.PatientId))
            .Select(b => new { b.PatientId, b.CreatedUtc }) // if CreatedUtc is varchar in DB, EF property type should be string
            .ToListAsync();

        // Helper to parse multiple formats safely
        static DateTime? ParseDate(object? val)
        {
            if (val is null) return null;
            var s = val.ToString();
            if (string.IsNullOrWhiteSpace(s)) return null;

            // Try ISO first, then common Indian/European formats
            string[] fmts = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy" };
            if (DateTime.TryParseExact(s, fmts, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeLocal, out var dt))
                return dt;

            // last resort
            if (DateTime.TryParse(s, out dt)) return dt;
            return null;
        }

        var lastApptByPatient = apptsRaw
            .GroupBy(a => a.PatientId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new { Dt = ParseDate(x.VisitDateTime), x.Reason })
                      .Where(x => x.Dt.HasValue)
                      .OrderByDescending(x => x.Dt.Value)
                      .FirstOrDefault()
            );

        var lastBillByPatient = billsRaw
            .GroupBy(b => b.PatientId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => ParseDate(x.CreatedUtc))
                      .Where(x => x.HasValue)
                      .Max()
            );

        var data = patients.Select(x =>
        {
            lastApptByPatient.TryGetValue(x.PatientId, out var appt);
            lastBillByPatient.TryGetValue(x.PatientId, out var billDt);

            return new PatientMasterDto
            {
                PatientId = x.PatientId,
                Mrn = x.Mrn,
                Phone = x.Phone,
                FullName = x.FullName,
                Status = x.Status,
                CreatedUtc = x.CreatedUtc,   // if these were varchar, you can also parse like above
                UpdatedUtc = x.UpdatedUtc,
                LatestVisitTime = appt?.Dt,
                LatestReason = appt?.Reason,
                Last_AppointmentDate = appt?.Dt,
                Last_BillingDate = billDt ?? default
            };
        }).ToList();

        return Ok(data);
    }


    // GET BY ID (edit popup)
    // Returns ONLY basic fields + last dates (no appointments/billings collections)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _db.Patients
            .Where(p => p.PatientId == id)
            .Select(p => new PatientDetailDto
            {
                PatientId = p.PatientId,
                FullName = p.FullName,
                Status = p.Status,                
                CreatedUtc = p.CreatedUtc,
                UpdatedUtc = p.UpdatedUtc,
                Mrn = p.Mrn,
                Dob = p.Dob,
                Gender = p.Gender,
                Phone = p.Phone,
                Address = p.Address

                //// last appointment date only (for display)
                //LatestVisitTime = _db.Appointments
                //    .Where(a => a.PatientId == p.PatientId)
                //    .Max(a => (DateTime?)a.VisitDateTime),

                //// last billing date only (for display)
                //Last_BillingDate = _db.Billings
                //    .Where(b => b.PatientId == p.PatientId)
                //    .Max(b => (DateTime?)b.CreatedUtc) ?? default
            })
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }

    // CREATE
    // PatientsController.cs  (API)
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] PatientCreateDto model)
    {
        if (await _db.Patients.AnyAsync(x => x.Mrn == model.Mrn))
            return Conflict($"MRN '{model.Mrn}' already exists.");

        var entity = new Patient
        {
            Mrn = model.Mrn,
            FullName = model.FullName,
            Dob = model.Dob,
            Gender = model.Gender,
            Phone = model.Phone,
            Address = model.Address,
            Status = model.Status,
            CreatedUtc = DateTime.UtcNow     
        };

        await _uow.Repository<Patient>().AddAsync(entity);
        await _uow.SaveAsync();
        
        return CreatedAtAction(nameof(Get), new { id = entity.PatientId }, entity.PatientId);
    }


    // UPDATE (basic-edit only)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDto model)
    {
        var repo = _uow.Repository<Patient>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        // map ONLY editable fields
        entity.FullName = model.FullName;
        entity.Dob = model.Dob;
        entity.Gender = model.Gender;
        entity.Phone = model.Phone;
        entity.Address = model.Address;
        entity.Status = model.Status;
        entity.UpdatedUtc = DateTime.UtcNow;

        repo.Update(entity);
        await _uow.SaveAsync();
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _uow.Repository<Patient>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        repo.Delete(entity);
        await _uow.SaveAsync();
        return NoContent();
    }
}

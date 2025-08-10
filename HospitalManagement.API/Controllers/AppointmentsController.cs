using HospitalManagement.API.Dtos;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly HospitalDbContext _db;

    public AppointmentsController(IUnitOfWork uow, HospitalDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    // GET: api/appointments
    // Master list — returns AppointmentMasterDto
    // GET: api/appointments
    // GET: api/appointments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Appointments
            .AsNoTracking()
            .Select(a => new AppointmentMasterDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,

                // entity VisitDateTime is non-nullable
                VisitDateTime = a.VisitDateTime,      // DTO is DateTime?; implicit upcast is fine

                Status = a.Status,
                IsEdit = !(a.Status != null && a.Status.Equals("Complete", StringComparison.OrdinalIgnoreCase)),

                // order by concrete DateTime; project as nullable
                LastVisitDateTime = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => (DateTime?)x.VisitDateTime)
                    .FirstOrDefault(),

                LastDoctorName = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => x.Doctor.FullName)
                    .FirstOrDefault(),

                Reason = a.Reason
            })
            .ToListAsync();

        return Ok(data);
    }



    // GET: api/appointments/5
    // Return a single AppointmentMasterDto by id

    // GET: api/appointments/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.AppointmentId == id)
            .Select(a => new AppointmentMasterDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,

                VisitDateTime = a.VisitDateTime,    // non-nullable entity -> nullable DTO

                Status = a.Status,
                IsEdit = !(a.Status != null && a.Status.Equals("Complete", StringComparison.OrdinalIgnoreCase)),

                LastVisitDateTime = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => (DateTime?)x.VisitDateTime)
                    .FirstOrDefault(),

                LastDoctorName = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => x.Doctor.FullName)
                    .FirstOrDefault(),

                Reason = a.Reason
            })
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }


    // GET: api/appointments/filter?patientId=12&doctorId=7&onlyOpen=true
    // Unified filter by patientId and/or doctorId; optional onlyOpen (Status != Complete)
    // GET: api/appointments/filter?patientId=12&doctorId=7&onlyOpen=true
    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] int? patientId, [FromQuery] int? doctorId, [FromQuery] bool onlyOpen = false)
    {
        var query = _db.Appointments.AsNoTracking().AsQueryable();

        if (patientId.HasValue) query = query.Where(a => a.PatientId == patientId.Value);
        if (doctorId.HasValue) query = query.Where(a => a.DoctorId == doctorId.Value);
        if (onlyOpen)
            query = query.Where(a => a.Status == null || !a.Status.Equals("Complete", StringComparison.OrdinalIgnoreCase));

        var list = await query
            .OrderByDescending(a => a.VisitDateTime) // concrete DateTime
            .Select(a => new AppointmentMasterDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,

                VisitDateTime = a.VisitDateTime,   // project as nullable

                Status = a.Status,
                IsEdit = !(a.Status != null && a.Status.Equals("Complete", StringComparison.OrdinalIgnoreCase)),

                LastVisitDateTime = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => (DateTime?)x.VisitDateTime)
                    .FirstOrDefault(),

                LastDoctorName = _db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.VisitDateTime)
                    .Select(x => x.Doctor.FullName)
                    .FirstOrDefault(),

                Reason = a.Reason
            })
            .ToListAsync();

        return Ok(list);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppointmentEditDto dto)
    {        

        var entity = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            VisitDateTime = dto.VisitDateTime,
            Status = dto.Status,
            Reason = dto.Reason,
            CreatedUtc = DateTime.UtcNow
        };

        await _uow.Repository<Appointment>().AddAsync(entity);
        await _uow.SaveAsync();
        return CreatedAtAction(nameof(Get), new { id = entity.AppointmentId }, entity.AppointmentId);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentEditDto dto)
    {
        if (id != dto.AppointmentId)
            return BadRequest("Route id and body id mismatch.");        

        var repo = _uow.Repository<Appointment>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        entity.PatientId = dto.PatientId;
        entity.DoctorId = dto.DoctorId;
        entity.VisitDateTime = dto.VisitDateTime;
        entity.Status = dto.Status;
        entity.Reason = dto.Reason;
        entity.UpdatedUtc = DateTime.UtcNow;

        await _uow.SaveAsync();
        return NoContent();
    }

    // DELETE: api/appointments/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _uow.Repository<Appointment>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        repo.Delete(entity);
        await _uow.SaveAsync();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure.Entities;
using HospitalManagement.API.Dtos;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public AppointmentsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/appointments
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _uow.Repository<Appointment>().GetAllAsync());

    // GET: api/appointments/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _uow.Repository<Appointment>().GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    // (Optional) GET: api/appointments/by-patient/12
    [HttpGet("by-patient/{patientId:int}")]
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var list = await _uow.Repository<Appointment>()
                              .FindAsync(a => a.PatientId == patientId);
        return Ok(list);
    }

    // (Optional) GET: api/appointments/by-doctor/7
    [HttpGet("by-doctor/{doctorId:int}")]
    public async Task<IActionResult> GetByDoctor(int doctorId)
    {
        var list = await _uow.Repository<Appointment>()
                              .FindAsync(a => a.DoctorId == doctorId);
        return Ok(list);
    }

    // POST: api/appointments
    [HttpPost]
    public async Task<IActionResult> Create(Appointment model)
    {
        await _uow.Repository<Appointment>().AddAsync(model);
        await _uow.SaveAsync();
        return CreatedAtAction(nameof(Get), new { id = model.AppointmentId }, model);
    }

    // PUT: api/appointments/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Appointment model)
    {
        if (id != model.AppointmentId) return BadRequest(); // adjust if PK name differs
        _uow.Repository<Appointment>().Update(model);
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

    [HttpGet("master")]
    public async Task<IActionResult> GetMaster([FromServices] HospitalManagement.Infrastructure.HospitalDbContext db)
    {
        var data = await db.Appointments
       .AsNoTracking()
       .Select(a => new AppointmentMasterDto
       {
           AppointmentId = a.AppointmentId,
           PatientId = a.PatientId,
           PatientName = a.Patient.FullName,
           DoctorId = a.DoctorId,
           DoctorName = a.Doctor.FullName,
           VisitDateTime = a.CreatedUtc,

           Status = a.Status,           
           IsEdit = !(a.Status != null &&
                                 a.Status.Equals("Complete", StringComparison.OrdinalIgnoreCase)),
           
           LastVisitDateTime = db.Appointments
               .Where(x => x.PatientId == a.PatientId)
               .OrderByDescending(x => x.CreatedUtc)
               .Select(x => (DateTime?)x.CreatedUtc)
               .FirstOrDefault(),

           LastDoctorName = db.Appointments
               .Where(x => x.PatientId == a.PatientId)
               .OrderByDescending(x => x.CreatedUtc)
               .Select(x => x.Doctor.FullName)
               .FirstOrDefault()
       })
       .ToListAsync();


        return Ok(data);
    }

    [HttpGet("{id:int}/details")]
    public async Task<IActionResult> GetDetails(int id, [FromServices] HospitalManagement.Infrastructure.HospitalDbContext db)
    {
        var dto = await db.Appointments
            .AsNoTracking()
            .Where(a => a.AppointmentId == id)
            .Select(a => new AppointmentDetailDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient.FullName,      // adjust if different
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,       // adjust if different
                VisitDateTime = a.CreatedUtc,

                // patient’s latest visit overall
                LastVisitDateTime = db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.CreatedUtc)
                    .Select(x => (DateTime?)x.CreatedUtc)
                    .FirstOrDefault(),

                LastDoctorName = db.Appointments
                    .Where(x => x.PatientId == a.PatientId)
                    .OrderByDescending(x => x.CreatedUtc)
                    .Select(x => x.Doctor.FullName)
                    .FirstOrDefault(),

                // latest history for THIS appointment (reason/type/details)
                Reason = db.Appointments
                    .Where(h => h.PatientId == a.PatientId)
                    .OrderByDescending(h => h.CreatedUtc)
                    .Select(h => h.Reason)                 // adjust property name if needed
                    .FirstOrDefault(),

                Type = db.PatientHistories
                    .Where(h => h.AppointmentId == a.AppointmentId)
                    .OrderByDescending(h => h.CreatedUtc)
                    .Select(h => h.Type)                   // adjust property name if needed
                    .FirstOrDefault(),

                Details = db.PatientHistories
                    .Where(h => h.AppointmentId == a.AppointmentId)
                    .OrderByDescending(h => h.CreatedUtc)
                    .Select(h => h.Details)                // adjust property name if needed
                    .FirstOrDefault(),

                // all attachments for THIS appointment (via history)
                Attachments = db.PatientHistoryAttachments
                    .Where(att => att.History.AppointmentId == a.AppointmentId)
                    .OrderByDescending(att => att.UploadedUtc)
                    .Select(att => new AttachmentDto
                    {
                        AttachmentId = att.AttachmentId,
                        FileName = att.FileName,       // adjust column name if different
                        UploadedUtc = att.UploadedUtc,
                        Notes = att.Notes           // adjust if you have a notes/desc column
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }
}

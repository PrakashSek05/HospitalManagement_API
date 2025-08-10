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
    public PatientsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _uow.Repository<Patient>().GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _uow.Repository<Patient>().GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Patient model)
    {
        await _uow.Repository<Patient>().AddAsync(model);
        await _uow.SaveAsync();
        return Ok(model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Patient model)
    {
        if (id != model.PatientId) return BadRequest(); // adjust to your key
        _uow.Repository<Patient>().Update(model);
        await _uow.SaveAsync();
        return NoContent();
    }

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

    [HttpGet("master")]
    public async Task<IActionResult> GetMaster([FromServices] HospitalDbContext db)
    {
        var data = await db.Patients
     .AsNoTracking()
     .Select(p => new
     {
         Patient = p,
         LatestAppointment = db.Appointments
             .Where(a => a.PatientId == p.PatientId)
             .OrderByDescending(a => a.CreatedUtc)
             .Select(a => new { a.CreatedUtc, a.Reason })
             .FirstOrDefault()
     })
     .Select(x => new PatientMasterDto
     {
         PatientId = x.Patient.PatientId,
         FullName = x.Patient.FullName,
         CreatedUtc = x.Patient.CreatedUtc,
         Status = x.Patient.Status,
         LatestVisitTime = x.LatestAppointment.CreatedUtc,
         LatestReason = x.LatestAppointment.Reason
     })
     .ToListAsync();


        return Ok(data);
    }
}

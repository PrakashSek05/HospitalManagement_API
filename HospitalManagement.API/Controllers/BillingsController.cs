using HospitalManagement.API.Dtos;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BillingsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly HospitalDbContext _db;

    public BillingsController(IUnitOfWork uow, HospitalDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    
    [HttpGet]
    public async Task<ActionResult<List<BillingMiniDto>>> GetAll()
    {
        var list = await _db.Billings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedUtc)
            .Select(b => new BillingMiniDto
            {
                BillingId = b.BillId,
                BillingDate = b.CreatedUtc,
                PatientName = b.Patient.FullName,
                AppointmentDate = b.Appointment != null
                                    ? b.Appointment.CreatedUtc /* <-- swap if needed */
                                    : (DateTime?)null,
                TotalAmount = b.TotalAmount,
                Discount = b.Discount,
                Tax = b.Tax
            })
            .ToListAsync();

        return Ok(list);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BillingDto>> Get(int id)
    {
        var dto = await _db.Billings
            .AsNoTracking()
            .Where(b => b.BillId == id)
            .Select(b => new BillingDto
            {
                BillingId = b.BillId,
                BillingDate = b.CreatedUtc,
                InvoiceNo = null, 
                TotalAmount = b.TotalAmount,
                Discount = b.Discount,
                Tax = b.Tax,
                NetAmount = b.NetAmount,
                Paid = b.PaidFlag,

                PatientId = b.PatientId,
                PatientName = b.Patient.FullName,
                AppointmentId = b.AppointmentId,
                AppointmentDate = b.Appointment != null
                                    ? b.Appointment.CreatedUtc 
                                    : (DateTime?)null
            })
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }

    
    [HttpGet("filter")]
    public async Task<ActionResult<List<BillingMiniDto>>> Filter(
        [FromQuery] int? patientId,
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] bool? paid)
    {
        var q = _db.Billings.AsNoTracking().AsQueryable();

        if (patientId.HasValue)
            q = q.Where(b => b.PatientId == patientId.Value);

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);
            q = q.Where(b => b.CreatedUtc >= start && b.CreatedUtc < end);
        }
        else
        {
            if (from.HasValue) q = q.Where(b => b.CreatedUtc >= from.Value.Date);
            if (to.HasValue) q = q.Where(b => b.CreatedUtc < to.Value.Date.AddDays(1));
        }

        if (paid.HasValue)
            q = q.Where(b => b.PaidFlag == paid.Value);

        var list = await q
            .OrderByDescending(b => b.CreatedUtc)
            .Select(b => new BillingMiniDto
            {
                BillingId = b.BillId,
                BillingDate = b.CreatedUtc,
                PatientName = b.Patient.FullName,
                AppointmentDate = b.Appointment != null
                                    ? b.Appointment.CreatedUtc /* <-- swap if needed */
                                    : (DateTime?)null,
                TotalAmount = b.TotalAmount,
                Discount = b.Discount,
                Tax = b.Tax
            })
            .ToListAsync();

        return Ok(list);
    }

    
    [HttpGet("by-patient/{patientId:int}")]
    public async Task<ActionResult<List<BillingMiniDto>>> ByPatient(int patientId, [FromQuery] DateTime? date)
    {
        var q = _db.Billings.AsNoTracking().Where(b => b.PatientId == patientId);

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);
            q = q.Where(b => b.CreatedUtc >= start && b.CreatedUtc < end);
        }

        var list = await q
            .OrderByDescending(b => b.CreatedUtc)
            .Select(b => new BillingMiniDto
            {
                BillingId = b.BillId,
                BillingDate = b.CreatedUtc,
                PatientName = b.Patient.FullName,
                AppointmentDate = b.Appointment != null
                                    ? b.Appointment.CreatedUtc /* <-- swap if needed */
                                    : (DateTime?)null,
                TotalAmount = b.TotalAmount,
                Discount = b.Discount,
                Tax = b.Tax
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BillingEditDto model)
    {
        if (model.PatientId <= 0)
            return BadRequest("PatientId is required.");

        var patientExists = await _db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.PatientId == model.PatientId);

        if (!patientExists)
            return BadRequest("Patient not found.");

        
        var latestApptId = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == model.PatientId)
            .OrderByDescending(a => a.CreatedUtc) 
            .Select(a => (int?)a.AppointmentId)
            .FirstOrDefaultAsync();

        var entity = new Billing
        {
            PatientId = model.PatientId,
            AppointmentId = latestApptId, 
            TotalAmount = model.TotalAmount,
            Discount = model.Discount,
            Tax = model.Tax,
            NetAmount = model.NetAmount,
            PaidFlag = model.PaidFlag,
            CreatedUtc = model.CreatedUtc == default ? DateTime.UtcNow : model.CreatedUtc
        };

        await _uow.Repository<Billing>().AddAsync(entity);
        await _uow.SaveAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.BillId }, entity.BillId);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BillingEditDto model)
    {
        if (id != model.BillId)
            return BadRequest("Route id does not match body BillId.");

        if (model.PatientId <= 0)
            return BadRequest("PatientId is required.");

        var repo = _uow.Repository<Billing>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        // Do NOT touch entity.AppointmentId
        entity.PatientId = model.PatientId;
        entity.TotalAmount = model.TotalAmount;
        entity.Discount = model.Discount;
        entity.Tax = model.Tax;
        entity.NetAmount = model.NetAmount;
        entity.PaidFlag = model.PaidFlag;
        entity.CreatedUtc = model.CreatedUtc;

        repo.Update(entity);
        await _uow.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _uow.Repository<Billing>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        repo.Delete(entity);
        await _uow.SaveAsync();
        return NoContent();
    }
}

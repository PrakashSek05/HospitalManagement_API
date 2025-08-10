using HospitalManagement.API.Dtos;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public DoctorsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _uow.Repository<Doctor>().GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _uow.Repository<Doctor>().GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Doctor model)
    {
        await _uow.Repository<Doctor>().AddAsync(model);
        await _uow.SaveAsync();
        return Ok(model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Doctor model)
    {
        if (id != model.DoctorId) return BadRequest(); // adjust PK if different
        _uow.Repository<Doctor>().Update(model);
        await _uow.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _uow.Repository<Doctor>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        repo.Delete(entity);
        await _uow.SaveAsync();
        return NoContent();
    }

    [HttpGet("master")]
    public async Task<IActionResult> GetMaster()
    {        
        var doctors = await _uow.Repository<Doctor>()
                                .GetAllAsync(d => d.Department, d => d.Specialties);

        var data = doctors.Select(d => new DoctorMasterDto
        {
            DoctorId = d.DoctorId,
            FullName = d.FullName,
            DepartmentName = d.Department != null ? d.Department.DepartmentName : "",
            Specialties = d.Specialties.Select(s => s.SpecialtyName).ToList(),
            IsAvailable = d.IsAvailable,
            CreatedUtc = d.CreatedUtc,
            UpdatedUtc = d.UpdatedUtc
        }).ToList();

        return Ok(data);
    }
}

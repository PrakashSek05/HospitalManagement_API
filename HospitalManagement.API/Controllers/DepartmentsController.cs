using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure.Entities;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public DepartmentsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/departments
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _uow.Repository<Department>().GetAllAsync());

    // GET: api/departments/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _uow.Repository<Department>().GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    // POST: api/departments
    [HttpPost]
    public async Task<IActionResult> Create(Department model)
    {
        await _uow.Repository<Department>().AddAsync(model);
        await _uow.SaveAsync();
        return CreatedAtAction(nameof(Get), new { id = model.DepartmentId }, model);
    }

    // PUT: api/departments/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Department model)
    {
        if (id != model.DepartmentId) return BadRequest();
        _uow.Repository<Department>().Update(model);
        await _uow.SaveAsync();
        return NoContent();
    }

    // DELETE: api/departments/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _uow.Repository<Department>();
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        repo.Delete(entity);
        await _uow.SaveAsync();
        return NoContent();
    }
}

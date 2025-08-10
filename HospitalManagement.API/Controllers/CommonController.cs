using HospitalManagement.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CommonController : ControllerBase
{
    private readonly ICommonService _svc;
    public CommonController(ICommonService svc) => _svc = svc;

    [HttpGet("specialties")]
    public async Task<ActionResult<List<DropdownItemDto>>> GetSpecialties(CancellationToken ct)
    {
        var list = await _svc.GetSpecialtiesAsync(ct);
        return Ok(list);
    }

    [HttpGet("departments")]
    public async Task<ActionResult<List<DropdownItemDto>>> GetDepartments(CancellationToken ct)
    {
        var list = await _svc.GetDepartmentsAsync(ct);
        return Ok(list);
    }

    [HttpGet("patients")]
    public async Task<ActionResult<List<DropdownItemDto>>> GetPatients(CancellationToken ct)
       => Ok(await _svc.GetPatientsAsync(ct));

    [HttpGet("doctors")]
    public async Task<ActionResult<List<DropdownItemDto>>> GetDoctors(CancellationToken ct)
        => Ok(await _svc.GetDoctorsAsync(ct));
}

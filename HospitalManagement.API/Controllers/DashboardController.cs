using HospitalManagement.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _svc;
    public DashboardController(IDashboardService svc) => _svc = svc;

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken ct)
    {
        var data = await _svc.GetSummaryAsync(ct);
        return Ok(data);
    }
}

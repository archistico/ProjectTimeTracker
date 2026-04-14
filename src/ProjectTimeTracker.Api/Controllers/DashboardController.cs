using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var result = await _service.GetDashboardAsync();
        return Ok(result);
    }
}
using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrgenzeController : ControllerBase
{
    private readonly IUrgenzeService _service;

    public UrgenzeController(IUrgenzeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UrgenzaDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
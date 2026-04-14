using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatiController : ControllerBase
{
    private readonly IStatiService _service;

    public StatiController(IStatiService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StatoDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AreeController : ControllerBase
{
    private readonly IAreeService _service;

    public AreeController(IAreeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<AreaDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
}
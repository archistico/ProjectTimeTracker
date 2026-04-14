using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CronologiaController : ControllerBase
{
    private readonly ICronologiaService _service;

    public CronologiaController(ICronologiaService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CronologiaDto>> Create([FromBody] CronologiaCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }
}
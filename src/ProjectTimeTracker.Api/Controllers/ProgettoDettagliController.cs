using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgettoDettagliController : ControllerBase
{
    private readonly IProgettoDettagliService _service;

    public ProgettoDettagliController(IProgettoDettagliService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ProgettoDettaglioDto>> Create([FromBody] ProgettoDettaglioCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }
}
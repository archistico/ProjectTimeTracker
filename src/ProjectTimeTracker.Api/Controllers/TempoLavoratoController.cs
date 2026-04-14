using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TempoLavoratoController : ControllerBase
{
    private readonly ITempoLavoratoService _service;

    public TempoLavoratoController(ITempoLavoratoService service)
    {
        _service = service;
    }

    [HttpGet("recenti")]
    public async Task<ActionResult<List<TempoLavoratoDto>>> GetRecenti([FromQuery] int take = 10)
    {
        var result = await _service.GetRecentiAsync(take);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TempoLavoratoDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TempoLavoratoDto>> Create([FromBody] TempoLavoratoCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TempoLavoratoUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
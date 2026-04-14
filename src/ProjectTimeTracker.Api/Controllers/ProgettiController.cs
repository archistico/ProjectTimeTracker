using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgettiController : ControllerBase
{
    private readonly IProgettiService _service;
    private readonly IProgettoDettagliService _dettagliService;
    private readonly ICronologiaService _cronologiaService;
    private readonly ITempoLavoratoService _tempoService;

    public ProgettiController(
        IProgettiService service,
        IProgettoDettagliService dettagliService,
        ICronologiaService cronologiaService,
        ITempoLavoratoService tempoService)
    {
        _service = service;
        _dettagliService = dettagliService;
        _cronologiaService = cronologiaService;
        _tempoService = tempoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProgettoDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProgettoDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProgettoDto>> Create([FromBody] ProgettoCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProgettoUpdateDto dto)
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

    [HttpGet("{id:int}/dettagli")]
    public async Task<ActionResult<List<ProgettoDettaglioDto>>> GetDettagli(int id)
    {
        var progetto = await _service.GetByIdAsync(id);
        if (progetto == null)
        {
            return NotFound();
        }

        return Ok(await _dettagliService.GetByProgettoIdAsync(id));
    }

    [HttpGet("{id:int}/cronologia")]
    public async Task<ActionResult<List<CronologiaDto>>> GetCronologia(int id)
    {
        var progetto = await _service.GetByIdAsync(id);
        if (progetto == null)
        {
            return NotFound();
        }

        return Ok(await _cronologiaService.GetByProgettoIdAsync(id));
    }

    [HttpGet("{id:int}/tempo")]
    public async Task<ActionResult<List<TempoLavoratoDto>>> GetTempo(int id)
    {
        var progetto = await _service.GetByIdAsync(id);
        if (progetto == null)
        {
            return NotFound();
        }

        return Ok(await _tempoService.GetByProgettoIdAsync(id));
    }

    [HttpGet("{id:int}/tempo/totale-minuti")]
    public async Task<ActionResult<int>> GetTotaleMinuti(int id)
    {
        var progetto = await _service.GetByIdAsync(id);
        if (progetto == null)
        {
            return NotFound();
        }

        return Ok(await _tempoService.GetTotaleMinutiByProgettoIdAsync(id));
    }
}
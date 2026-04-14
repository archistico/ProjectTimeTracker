using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

/// <summary>
/// Espone gli endpoint REST per la gestione dei progetti e delle relative informazioni collegate.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProgettiController : ControllerBase
{
    private readonly IProgettiService _service;
    private readonly IProgettoDettagliService _dettagliService;
    private readonly ICronologiaService _cronologiaService;
    private readonly ITempoLavoratoService _tempoService;

    /// <summary>
    /// Inizializza una nuova istanza del controller.
    /// </summary>
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

    /// <summary>
    /// Restituisce tutti i progetti.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProgettoDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Restituisce un progetto per identificativo.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProgettoDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Crea un nuovo progetto.
    /// </summary>
    /// <param name="dto">Dati del progetto da creare.</param>
    [HttpPost]
    public async Task<ActionResult<ProgettoDto>> Create([FromBody] ProgettoCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Aggiorna un progetto esistente.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    /// <param name="dto">Nuovi dati del progetto.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProgettoUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Elimina un progetto esistente.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Restituisce i dettagli di un progetto.
    /// La verifica di esistenza del progetto è demandata al service.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpGet("{id:int}/dettagli")]
    public async Task<ActionResult<List<ProgettoDettaglioDto>>> GetDettagli(int id)
    {
        return Ok(await _dettagliService.GetByProgettoIdAsync(id));
    }

    /// <summary>
    /// Restituisce la cronologia di un progetto.
    /// La verifica di esistenza del progetto è demandata al service.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpGet("{id:int}/cronologia")]
    public async Task<ActionResult<List<CronologiaDto>>> GetCronologia(int id)
    {
        return Ok(await _cronologiaService.GetByProgettoIdAsync(id));
    }

    /// <summary>
    /// Restituisce i tempi lavorati di un progetto.
    /// La verifica di esistenza del progetto è demandata al service.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpGet("{id:int}/tempo")]
    public async Task<ActionResult<List<TempoLavoratoDto>>> GetTempo(int id)
    {
        return Ok(await _tempoService.GetByProgettoIdAsync(id));
    }

    /// <summary>
    /// Restituisce il totale dei minuti lavorati per un progetto.
    /// La verifica di esistenza del progetto è demandata al service.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    [HttpGet("{id:int}/tempo/totale-minuti")]
    public async Task<ActionResult<int>> GetTotaleMinuti(int id)
    {
        return Ok(await _tempoService.GetTotaleMinutiByProgettoIdAsync(id));
    }
}
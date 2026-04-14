using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

/// <summary>
/// Espone gli endpoint REST per la gestione della cronologia dei progetti.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CronologiaController : ControllerBase
{
    private readonly ICronologiaService _service;

    /// <summary>
    /// Inizializza una nuova istanza del controller.
    /// </summary>
    /// <param name="service">Service della cronologia.</param>
    public CronologiaController(ICronologiaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Restituisce una voce di cronologia per identificativo.
    /// </summary>
    /// <param name="id">Identificativo della voce di cronologia.</param>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CronologiaDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Crea una nuova voce di cronologia.
    /// </summary>
    /// <param name="dto">Dati della voce da creare.</param>
    [HttpPost]
    public async Task<ActionResult<CronologiaDto>> Create([FromBody] CronologiaCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
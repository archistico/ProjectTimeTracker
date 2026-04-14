using Microsoft.AspNetCore.Mvc;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtentiController : ControllerBase
{
    private readonly IUtentiService _service;

    public UtentiController(IUtentiService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UtenteDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UtenteDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UtenteDto>> Create([FromBody] UtenteCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
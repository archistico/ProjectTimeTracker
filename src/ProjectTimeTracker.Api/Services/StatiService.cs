using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la consultazione degli stati.
/// </summary>
public class StatiService : IStatiService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public StatiService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce l'elenco completo degli stati ordinati per identificativo.
    /// </summary>
    public async Task<List<StatoDto>> GetAllAsync()
    {
        return await _db.Stati
            .OrderBy(x => x.Id)
            .Select(x => new StatoDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}
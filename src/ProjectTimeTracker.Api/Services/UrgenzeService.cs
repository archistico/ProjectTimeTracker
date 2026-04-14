using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la consultazione delle urgenze.
/// </summary>
public class UrgenzeService : IUrgenzeService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public UrgenzeService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce l'elenco completo delle urgenze ordinate per identificativo.
    /// </summary>
    public async Task<List<UrgenzaDto>> GetAllAsync()
    {
        return await _db.Urgenze
            .OrderBy(x => x.Id)
            .Select(x => new UrgenzaDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}
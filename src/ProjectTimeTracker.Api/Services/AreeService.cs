using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la consultazione delle aree.
/// </summary>
public class AreeService : IAreeService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public AreeService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce l'elenco completo delle aree ordinate per descrizione.
    /// </summary>
    public async Task<List<AreaDto>> GetAllAsync()
    {
        return await _db.Aree
            .OrderBy(x => x.Descrizione)
            .Select(x => new AreaDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la gestione della cronologia.
/// </summary>
public class CronologiaService : ICronologiaService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public CronologiaService(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<CronologiaDto?> GetByIdAsync(int id)
    {
        return await _db.Cronologie
            .Include(x => x.Utente)
            .Where(x => x.Id == id)
            .Select(x => new CronologiaDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? $"{x.Utente.Cognome} {x.Utente.Nome}" : string.Empty,
                Azione = x.Azione,
                Data = x.Data
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<List<CronologiaDto>> GetByProgettoIdAsync(int progettoId)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == progettoId))
        {
            throw new EntityNotFoundException("Progetto", progettoId);
        }

        return await _db.Cronologie
            .Include(x => x.Utente)
            .Where(x => x.ProgettoId == progettoId)
            .OrderByDescending(x => x.Data)
            .Select(x => new CronologiaDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? $"{x.Utente.Cognome} {x.Utente.Nome}" : string.Empty,
                Azione = x.Azione,
                Data = x.Data
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<CronologiaDto> CreateAsync(CronologiaCreateDto dto)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == dto.ProgettoId))
        {
            throw new EntityNotFoundException("Progetto", dto.ProgettoId);
        }

        if (!await _db.Utenti.AnyAsync(x => x.Id == dto.UtenteId))
        {
            throw new EntityNotFoundException("Utente", dto.UtenteId);
        }

        var entity = new Cronologia
        {
            ProgettoId = dto.ProgettoId,
            UtenteId = dto.UtenteId,
            Azione = dto.Azione,
            Data = dto.Data
        };

        _db.Cronologie.Add(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
               ?? throw new InvalidOperationException("Voce di cronologia non trovata dopo il salvataggio.");
    }
}
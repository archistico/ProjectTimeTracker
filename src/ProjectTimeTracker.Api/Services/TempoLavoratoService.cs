using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la gestione dei tempi lavorati.
/// </summary>
public class TempoLavoratoService : ITempoLavoratoService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public TempoLavoratoService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce tutti i tempi lavorati associati a un progetto.
    /// </summary>
    /// <param name="progettoId">Identificativo del progetto.</param>
    public async Task<List<TempoLavoratoDto>> GetByProgettoIdAsync(int progettoId)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == progettoId))
        {
            throw new EntityNotFoundException("Progetto", progettoId);
        }

        return await _db.TempiLavorati
            .Include(x => x.Utente)
            .Where(x => x.ProgettoId == progettoId)
            .OrderByDescending(x => x.Data)
            .Select(x => new TempoLavoratoDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? $"{x.Utente.Cognome} {x.Utente.Nome}" : string.Empty,
                Data = x.Data,
                Minuti = x.Minuti,
                Nota = x.Nota
            })
            .ToListAsync();
    }

    /// <summary>
    /// Restituisce gli ultimi tempi lavorati registrati.
    /// </summary>
    /// <param name="take">Numero massimo di record da restituire.</param>
    public async Task<List<TempoLavoratoDto>> GetRecentiAsync(int take)
    {
        if (take <= 0)
        {
            take = 10;
        }

        return await _db.TempiLavorati
            .Include(x => x.Utente)
            .Include(x => x.Progetto)
            .OrderByDescending(x => x.Data)
            .Take(take)
            .Select(x => new TempoLavoratoDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? $"{x.Utente.Cognome} {x.Utente.Nome}" : string.Empty,
                Data = x.Data,
                Minuti = x.Minuti,
                Nota = x.Nota
            })
            .ToListAsync();
    }

    /// <summary>
    /// Restituisce il totale dei minuti lavorati per un progetto.
    /// </summary>
    /// <param name="progettoId">Identificativo del progetto.</param>
    public async Task<int> GetTotaleMinutiByProgettoIdAsync(int progettoId)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == progettoId))
        {
            throw new EntityNotFoundException("Progetto", progettoId);
        }

        return await _db.TempiLavorati
            .Where(x => x.ProgettoId == progettoId)
            .SumAsync(x => (int?)x.Minuti) ?? 0;
    }

    /// <summary>
    /// Restituisce un record di tempo lavorato per identificativo.
    /// </summary>
    /// <param name="id">Identificativo del record.</param>
    public async Task<TempoLavoratoDto?> GetByIdAsync(int id)
    {
        return await _db.TempiLavorati
            .Include(x => x.Utente)
            .Where(x => x.Id == id)
            .Select(x => new TempoLavoratoDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? $"{x.Utente.Cognome} {x.Utente.Nome}" : string.Empty,
                Data = x.Data,
                Minuti = x.Minuti,
                Nota = x.Nota
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Crea un nuovo record di tempo lavorato.
    /// </summary>
    /// <param name="dto">Dati del record da creare.</param>
    public async Task<TempoLavoratoDto> CreateAsync(TempoLavoratoCreateDto dto)
    {
        await VerificaRiferimentiAsync(dto.ProgettoId, dto.UtenteId);

        var entity = new TempoLavorato
        {
            ProgettoId = dto.ProgettoId,
            UtenteId = dto.UtenteId,
            Data = dto.Data,
            Minuti = dto.Minuti,
            Nota = dto.Nota
        };

        _db.TempiLavorati.Add(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
               ?? throw new InvalidOperationException("Tempo lavorato non trovato dopo il salvataggio.");
    }

    /// <summary>
    /// Aggiorna un record di tempo lavorato esistente.
    /// </summary>
    /// <param name="id">Identificativo del record.</param>
    /// <param name="dto">Nuovi dati del record.</param>
    public async Task<bool> UpdateAsync(int id, TempoLavoratoUpdateDto dto)
    {
        var entity = await _db.TempiLavorati.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        await VerificaRiferimentiAsync(dto.ProgettoId, dto.UtenteId);

        entity.ProgettoId = dto.ProgettoId;
        entity.UtenteId = dto.UtenteId;
        entity.Data = dto.Data;
        entity.Minuti = dto.Minuti;
        entity.Nota = dto.Nota;

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina un record di tempo lavorato.
    /// </summary>
    /// <param name="id">Identificativo del record.</param>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.TempiLavorati.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        _db.TempiLavorati.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Verifica l'esistenza delle entità referenziate dal tempo lavorato.
    /// </summary>
    private async Task VerificaRiferimentiAsync(int progettoId, int utenteId)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == progettoId))
        {
            throw new EntityNotFoundException("Progetto", progettoId);
        }

        if (!await _db.Utenti.AnyAsync(x => x.Id == utenteId))
        {
            throw new EntityNotFoundException("Utente", utenteId);
        }
    }
}
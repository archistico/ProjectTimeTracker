using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la gestione dei progetti.
/// </summary>
public class ProgettiService : IProgettiService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public ProgettiService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce tutti i progetti con i relativi dati descrittivi e il totale minuti lavorati.
    /// </summary>
    public async Task<List<ProgettoDto>> GetAllAsync()
    {
        return await _db.Progetti
            .Include(x => x.Area)
            .Include(x => x.Stato)
            .Include(x => x.Urgenza)
            .Include(x => x.TempiLavorati)
            .OrderByDescending(x => x.DataInizio)
            .Select(x => new ProgettoDto
            {
                Id = x.Id,
                Oggetto = x.Oggetto,
                Descrizione = x.Descrizione,
                AreaId = x.AreaId,
                Area = x.Area != null ? x.Area.Descrizione : string.Empty,
                StatoId = x.StatoId,
                Stato = x.Stato != null ? x.Stato.Descrizione : string.Empty,
                UrgenzaId = x.UrgenzaId,
                Urgenza = x.Urgenza != null ? x.Urgenza.Descrizione : string.Empty,
                DataInizio = x.DataInizio,
                DataFineRichiesta = x.DataFineRichiesta,
                DataFineEffettiva = x.DataFineEffettiva,
                TotaleMinutiLavorati = x.TempiLavorati.Sum(t => t.Minuti)
            })
            .ToListAsync();
    }

    /// <summary>
    /// Restituisce un progetto per identificativo.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    public async Task<ProgettoDto?> GetByIdAsync(int id)
    {
        return await _db.Progetti
            .Include(x => x.Area)
            .Include(x => x.Stato)
            .Include(x => x.Urgenza)
            .Include(x => x.TempiLavorati)
            .Where(x => x.Id == id)
            .Select(x => new ProgettoDto
            {
                Id = x.Id,
                Oggetto = x.Oggetto,
                Descrizione = x.Descrizione,
                AreaId = x.AreaId,
                Area = x.Area != null ? x.Area.Descrizione : string.Empty,
                StatoId = x.StatoId,
                Stato = x.Stato != null ? x.Stato.Descrizione : string.Empty,
                UrgenzaId = x.UrgenzaId,
                Urgenza = x.Urgenza != null ? x.Urgenza.Descrizione : string.Empty,
                DataInizio = x.DataInizio,
                DataFineRichiesta = x.DataFineRichiesta,
                DataFineEffettiva = x.DataFineEffettiva,
                TotaleMinutiLavorati = x.TempiLavorati.Sum(t => t.Minuti)
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Crea un nuovo progetto dopo aver validato i riferimenti collegati.
    /// </summary>
    /// <param name="dto">Dati del progetto da creare.</param>
    public async Task<ProgettoDto> CreateAsync(ProgettoCreateDto dto)
    {
        await VerificaRiferimentiAsync(dto.AreaId, dto.StatoId, dto.UrgenzaId);

        var entity = new Progetto
        {
            Oggetto = dto.Oggetto,
            Descrizione = dto.Descrizione,
            AreaId = dto.AreaId,
            StatoId = dto.StatoId,
            UrgenzaId = dto.UrgenzaId,
            DataInizio = dto.DataInizio,
            DataFineRichiesta = dto.DataFineRichiesta
        };

        _db.Progetti.Add(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
               ?? throw new InvalidOperationException("Progetto non trovato dopo il salvataggio.");
    }

    /// <summary>
    /// Aggiorna un progetto esistente.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    /// <param name="dto">Nuovi dati del progetto.</param>
    public async Task<bool> UpdateAsync(int id, ProgettoUpdateDto dto)
    {
        await VerificaRiferimentiAsync(dto.AreaId, dto.StatoId, dto.UrgenzaId);

        var entity = await _db.Progetti.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        entity.Oggetto = dto.Oggetto;
        entity.Descrizione = dto.Descrizione;
        entity.AreaId = dto.AreaId;
        entity.StatoId = dto.StatoId;
        entity.UrgenzaId = dto.UrgenzaId;
        entity.DataInizio = dto.DataInizio;
        entity.DataFineRichiesta = dto.DataFineRichiesta;
        entity.DataFineEffettiva = dto.DataFineEffettiva;

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina un progetto esistente.
    /// </summary>
    /// <param name="id">Identificativo del progetto.</param>
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Progetti.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        _db.Progetti.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Verifica l'esistenza delle entità referenziate dal progetto.
    /// </summary>
    private async Task VerificaRiferimentiAsync(int areaId, int statoId, int urgenzaId)
    {
        if (!await _db.Aree.AnyAsync(x => x.Id == areaId))
        {
            throw new EntityNotFoundException("Area", areaId);
        }

        if (!await _db.Stati.AnyAsync(x => x.Id == statoId))
        {
            throw new EntityNotFoundException("Stato", statoId);
        }

        if (!await _db.Urgenze.AnyAsync(x => x.Id == urgenzaId))
        {
            throw new EntityNotFoundException("Urgenza", urgenzaId);
        }
    }
}
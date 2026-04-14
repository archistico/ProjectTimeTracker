using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

public class TempoLavoratoService : ITempoLavoratoService
{
    private readonly AppDbContext _db;

    public TempoLavoratoService(AppDbContext db)
    {
        _db = db;
    }

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

    public async Task<TempoLavoratoDto> CreateAsync(TempoLavoratoCreateDto dto)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == dto.ProgettoId))
        {
            throw new EntityNotFoundException("Progetto", dto.ProgettoId);
        }

        if (!await _db.Utenti.AnyAsync(x => x.Id == dto.UtenteId))
        {
            throw new EntityNotFoundException("Utente", dto.UtenteId);
        }

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

    public async Task<bool> UpdateAsync(int id, TempoLavoratoUpdateDto dto)
    {
        var entity = await _db.TempiLavorati.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        entity.Data = dto.Data;
        entity.Minuti = dto.Minuti;
        entity.Nota = dto.Nota;

        await _db.SaveChangesAsync();
        return true;
    }

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
}
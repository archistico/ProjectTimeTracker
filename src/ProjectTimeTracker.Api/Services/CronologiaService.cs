using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

public class CronologiaService : ICronologiaService
{
    private readonly AppDbContext _db;

    public CronologiaService(AppDbContext db)
    {
        _db = db;
    }

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
                Utente = x.Utente != null ? x.Utente.Cognome + " " + x.Utente.Nome : string.Empty,
                Azione = x.Azione,
                Data = x.Data
            })
            .ToListAsync();
    }

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

        return await _db.Cronologie
            .Include(x => x.Utente)
            .Where(x => x.Id == entity.Id)
            .Select(x => new CronologiaDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? x.Utente.Cognome + " " + x.Utente.Nome : string.Empty,
                Azione = x.Azione,
                Data = x.Data
            })
            .FirstAsync();
    }
}
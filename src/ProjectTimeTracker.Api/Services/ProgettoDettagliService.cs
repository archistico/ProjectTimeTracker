using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

public class ProgettoDettagliService : IProgettoDettagliService
{
    private readonly AppDbContext _db;

    public ProgettoDettagliService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProgettoDettaglioDto>> GetByProgettoIdAsync(int progettoId)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == progettoId))
        {
            throw new EntityNotFoundException("Progetto", progettoId);
        }

        return await _db.ProgettoDettagli
            .Include(x => x.Utente)
            .Where(x => x.ProgettoId == progettoId)
            .OrderByDescending(x => x.Data)
            .Select(x => new ProgettoDettaglioDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                Dettaglio = x.Dettaglio,
                Data = x.Data,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? x.Utente.Cognome + " " + x.Utente.Nome : string.Empty
            })
            .ToListAsync();
    }

    public async Task<ProgettoDettaglioDto> CreateAsync(ProgettoDettaglioCreateDto dto)
    {
        if (!await _db.Progetti.AnyAsync(x => x.Id == dto.ProgettoId))
        {
            throw new EntityNotFoundException("Progetto", dto.ProgettoId);
        }

        if (!await _db.Utenti.AnyAsync(x => x.Id == dto.UtenteId))
        {
            throw new EntityNotFoundException("Utente", dto.UtenteId);
        }

        var entity = new ProgettoDettaglio
        {
            ProgettoId = dto.ProgettoId,
            Dettaglio = dto.Dettaglio,
            Data = dto.Data,
            UtenteId = dto.UtenteId
        };

        _db.ProgettoDettagli.Add(entity);
        await _db.SaveChangesAsync();

        return await _db.ProgettoDettagli
            .Include(x => x.Utente)
            .Where(x => x.Id == entity.Id)
            .Select(x => new ProgettoDettaglioDto
            {
                Id = x.Id,
                ProgettoId = x.ProgettoId,
                Dettaglio = x.Dettaglio,
                Data = x.Data,
                UtenteId = x.UtenteId,
                Utente = x.Utente != null ? x.Utente.Cognome + " " + x.Utente.Nome : string.Empty
            })
            .FirstAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

public class UtentiService : IUtentiService
{
    private readonly AppDbContext _db;

    public UtentiService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UtenteDto>> GetAllAsync()
    {
        return await _db.Utenti
            .OrderBy(x => x.Cognome)
            .ThenBy(x => x.Nome)
            .Select(x => new UtenteDto
            {
                Id = x.Id,
                Username = x.Username,
                Cognome = x.Cognome,
                Nome = x.Nome
            })
            .ToListAsync();
    }

    public async Task<UtenteDto?> GetByIdAsync(int id)
    {
        return await _db.Utenti
            .Where(x => x.Id == id)
            .Select(x => new UtenteDto
            {
                Id = x.Id,
                Username = x.Username,
                Cognome = x.Cognome,
                Nome = x.Nome
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UtenteDto> CreateAsync(UtenteCreateDto dto)
    {
        var entity = new Utente
        {
            Username = dto.Username,
            Cognome = dto.Cognome,
            Nome = dto.Nome
        };

        _db.Utenti.Add(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id) ?? throw new InvalidOperationException("Utente non trovato dopo il salvataggio.");
    }
}
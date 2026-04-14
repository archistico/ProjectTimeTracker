using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per la gestione degli utenti.
/// </summary>
public class UtentiService : IUtentiService
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public UtentiService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Restituisce tutti gli utenti ordinati per cognome e nome.
    /// </summary>
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

    /// <summary>
    /// Restituisce un utente per identificativo.
    /// </summary>
    /// <param name="id">Identificativo dell'utente.</param>
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

    /// <summary>
    /// Crea un nuovo utente.
    /// </summary>
    /// <param name="dto">Dati dell'utente da creare.</param>
    public async Task<UtenteDto> CreateAsync(UtenteCreateDto dto)
    {
        var entity = new Utente
        {
            Username = dto.Username.Trim(),
            Cognome = dto.Cognome.Trim(),
            Nome = dto.Nome.Trim()
        };

        _db.Utenti.Add(entity);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
               ?? throw new InvalidOperationException("Utente non trovato dopo il salvataggio.");
    }
}
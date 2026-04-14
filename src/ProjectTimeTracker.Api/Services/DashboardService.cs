using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Implementa la logica applicativa per il cruscotto riepilogativo.
/// </summary>
public class DashboardService : IDashboardService
{
    private const string StatoCompletato = "Completato";
    private const string StatoAnnullato = "Annullato";
    private const string UrgenzaUrgente = "Urgente";

    private readonly IAppDbContext _db;

    /// <summary>
    /// Inizializza una nuova istanza del service.
    /// </summary>
    /// <param name="db">Contesto applicativo astratto.</param>
    public DashboardService(IAppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Costruisce i dati aggregati della dashboard.
    /// </summary>
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var oggi = DateTime.Today;
        var setteGiorniFa = oggi.AddDays(-6);

        var progettiAttiviQuery = GetProgettiAttiviQuery();

        var progettiAperti = await progettiAttiviQuery.CountAsync();

        var progettiUrgenti = await progettiAttiviQuery
            .Where(x => x.Urgenza != null && x.Urgenza.Descrizione == UrgenzaUrgente)
            .CountAsync();

        var progettiInRitardo = await progettiAttiviQuery
            .Where(x => x.DataFineRichiesta.HasValue && x.DataFineRichiesta.Value.Date < oggi)
            .CountAsync();

        var minutiLavoratiOggi = await _db.TempiLavorati
            .Where(x => x.Data.Date == oggi)
            .SumAsync(x => (int?)x.Minuti) ?? 0;

        var minutiLavoratiUltimi7Giorni = await _db.TempiLavorati
            .Where(x => x.Data.Date >= setteGiorniFa && x.Data.Date <= oggi)
            .SumAsync(x => (int?)x.Minuti) ?? 0;

        var ultimiAggiornamenti = await _db.Cronologie
            .Include(x => x.Utente)
            .Where(x => x.Progetto != null)
            .OrderByDescending(x => x.Data)
            .Take(10)
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

        return new DashboardDto
        {
            ProgettiAperti = progettiAperti,
            ProgettiUrgenti = progettiUrgenti,
            ProgettiInRitardo = progettiInRitardo,
            MinutiLavoratiOggi = minutiLavoratiOggi,
            MinutiLavoratiUltimi7Giorni = minutiLavoratiUltimi7Giorni,
            UltimiAggiornamenti = ultimiAggiornamenti
        };
    }

    /// <summary>
    /// Costruisce la query base dei progetti considerati attivi.
    /// Un progetto è considerato attivo se non ha una data di fine effettiva
    /// e non è in uno stato finale noto.
    /// </summary>
    private IQueryable<ProjectTimeTracker.Api.Models.Progetto> GetProgettiAttiviQuery()
    {
        return _db.Progetti.Where(x =>
            !x.DataFineEffettiva.HasValue &&
            (x.Stato == null ||
             (x.Stato.Descrizione != StatoCompletato && x.Stato.Descrizione != StatoAnnullato)));
    }
}
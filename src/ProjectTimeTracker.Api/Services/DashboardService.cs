using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var oggi = DateTime.Today;
        var setteGiorniFa = oggi.AddDays(-6);

        var progettoCompletato = await _db.Stati
            .Where(x => x.Descrizione == "Completato")
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();

        var progettiApertiQuery = _db.Progetti.AsQueryable();

        if (progettoCompletato.HasValue)
        {
            progettiApertiQuery = progettiApertiQuery.Where(x => x.StatoId != progettoCompletato.Value);
        }

        var progettiAperti = await progettiApertiQuery.CountAsync();

        var urgenzaUrgenteId = await _db.Urgenze
            .Where(x => x.Descrizione == "Urgente")
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();

        var progettiUrgentiQuery = _db.Progetti.AsQueryable();
        if (urgenzaUrgenteId.HasValue)
        {
            progettiUrgentiQuery = progettiUrgentiQuery.Where(x => x.UrgenzaId == urgenzaUrgenteId.Value);
        }

        if (progettoCompletato.HasValue)
        {
            progettiUrgentiQuery = progettiUrgentiQuery.Where(x => x.StatoId != progettoCompletato.Value);
        }

        var progettiUrgenti = await progettiUrgentiQuery.CountAsync();

        var progettiInRitardoQuery = _db.Progetti
            .Where(x => x.DataFineRichiesta.HasValue &&
                        x.DataFineRichiesta.Value.Date < oggi);

        if (progettoCompletato.HasValue)
        {
            progettiInRitardoQuery = progettiInRitardoQuery.Where(x => x.StatoId != progettoCompletato.Value);
        }

        var progettiInRitardo = await progettiInRitardoQuery.CountAsync();

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
}
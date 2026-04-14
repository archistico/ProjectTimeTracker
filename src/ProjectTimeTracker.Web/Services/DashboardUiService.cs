using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public class DashboardUiService : IDashboardUiService
{
    private sealed class DashboardApiResponse
    {
        public int ProgettiAperti { get; set; }
        public int ProgettiUrgenti { get; set; }
        public int ProgettiInRitardo { get; set; }
        public int MinutiLavoratiOggi { get; set; }
        public int MinutiLavoratiUltimi7Giorni { get; set; }
        public List<CronologiaApiItem> UltimiAggiornamenti { get; set; } = new();
    }

    private sealed class CronologiaApiItem
    {
        public int Id { get; set; }
        public int ProgettoId { get; set; }
        public int UtenteId { get; set; }
        public string Utente { get; set; } = string.Empty;
        public string Azione { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }

    private readonly IApiClient _apiClient;

    public DashboardUiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetAsync<DashboardApiResponse>(ApiEndpoints.Dashboard, cancellationToken)
                         ?? new DashboardApiResponse();

            return new DashboardViewModel
            {
                ProgettiAperti = new DashboardStatCardViewModel
                {
                    Titolo = "Progetti aperti",
                    Valore = response.ProgettiAperti.ToString(),
                    Sottotitolo = "Attualmente non completati"
                },
                ProgettiUrgenti = new DashboardStatCardViewModel
                {
                    Titolo = "Progetti urgenti",
                    Valore = response.ProgettiUrgenti.ToString(),
                    Sottotitolo = "Da tenere sotto controllo"
                },
                ProgettiInRitardo = new DashboardStatCardViewModel
                {
                    Titolo = "Progetti in ritardo",
                    Valore = response.ProgettiInRitardo.ToString(),
                    Sottotitolo = "Con data richiesta superata"
                },
                TempoOggi = new DashboardStatCardViewModel
                {
                    Titolo = "Tempo lavorato oggi",
                    Valore = FormatMinutes(response.MinutiLavoratiOggi),
                    Sottotitolo = "Somma di tutte le registrazioni odierne"
                },
                TempoUltimi7Giorni = new DashboardStatCardViewModel
                {
                    Titolo = "Ultimi 7 giorni",
                    Valore = FormatMinutes(response.MinutiLavoratiUltimi7Giorni),
                    Sottotitolo = "Tempo complessivo settimanale"
                },
                UltimiAggiornamenti = response.UltimiAggiornamenti
                    .OrderByDescending(x => x.Data)
                    .Select(x => new RecentActivityViewModel
                    {
                        Data = x.Data,
                        Azione = x.Azione,
                        Utente = x.Utente,
                        ProgettoId = x.ProgettoId
                    })
                    .ToList()
            };
        }
        catch (ApiClientException ex)
        {
            return CreateErrorDashboard(ex.Message);
        }
    }

    public string FormatMinutes(int minutes)
    {
        var ore = minutes / 60;
        var minuti = minutes % 60;
        return $"{ore}h {minuti}m";
    }

    private DashboardViewModel CreateErrorDashboard(string message)
    {
        return new DashboardViewModel
        {
            ProgettiAperti = CreateEmptyCard("Progetti aperti"),
            ProgettiUrgenti = CreateEmptyCard("Progetti urgenti"),
            ProgettiInRitardo = CreateEmptyCard("Progetti in ritardo"),
            TempoOggi = CreateEmptyCard("Tempo lavorato oggi"),
            TempoUltimi7Giorni = CreateEmptyCard("Ultimi 7 giorni"),
            UltimiAggiornamenti = new List<RecentActivityViewModel>(),
            Message = new UiMessageViewModel
            {
                Title = "Dashboard non disponibile",
                Text = message,
                CssClass = "alert-danger"
            }
        };
    }

    private static DashboardStatCardViewModel CreateEmptyCard(string titolo)
    {
        return new DashboardStatCardViewModel
        {
            Titolo = titolo,
            Valore = "-",
            Sottotitolo = "Dato non disponibile"
        };
    }
}
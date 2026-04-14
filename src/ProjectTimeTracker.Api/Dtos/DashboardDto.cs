namespace ProjectTimeTracker.Api.Dtos;

public class DashboardDto
{
    public int ProgettiAperti { get; set; }
    public int ProgettiUrgenti { get; set; }
    public int ProgettiInRitardo { get; set; }
    public int MinutiLavoratiOggi { get; set; }
    public int MinutiLavoratiUltimi7Giorni { get; set; }
    public List<CronologiaDto> UltimiAggiornamenti { get; set; } = new();
}
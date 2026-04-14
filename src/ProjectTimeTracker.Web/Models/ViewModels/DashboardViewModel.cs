namespace ProjectTimeTracker.Web.Models.ViewModels;

public class DashboardViewModel
{
    public DashboardStatCardViewModel ProgettiAperti { get; set; } = new();
    public DashboardStatCardViewModel ProgettiUrgenti { get; set; } = new();
    public DashboardStatCardViewModel ProgettiInRitardo { get; set; } = new();
    public DashboardStatCardViewModel TempoOggi { get; set; } = new();
    public DashboardStatCardViewModel TempoUltimi7Giorni { get; set; } = new();

    public List<RecentActivityViewModel> UltimiAggiornamenti { get; set; } = new();
}
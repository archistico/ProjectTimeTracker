namespace ProjectTimeTracker.Web.Models.ViewModels;

public class RecentActivityViewModel
{
    public DateTime Data { get; set; }
    public string Azione { get; set; } = string.Empty;
    public string Utente { get; set; } = string.Empty;
    public int ProgettoId { get; set; }
}
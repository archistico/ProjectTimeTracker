namespace ProjectTimeTracker.Web.Models.ViewModels;

public class CurrentUserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string DisplayName => $"{Cognome} {Nome}".Trim();
}
namespace ProjectTimeTracker.Api.Models;

public class Cronologia
{
    public int Id { get; set; }

    public int ProgettoId { get; set; }
    public Progetto? Progetto { get; set; }

    public int UtenteId { get; set; }
    public Utente? Utente { get; set; }

    public string Azione { get; set; } = string.Empty;
    public DateTime Data { get; set; }
}
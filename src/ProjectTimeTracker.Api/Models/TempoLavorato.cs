namespace ProjectTimeTracker.Api.Models;

public class TempoLavorato
{
    public int Id { get; set; }

    public int ProgettoId { get; set; }
    public Progetto? Progetto { get; set; }

    public int UtenteId { get; set; }
    public Utente? Utente { get; set; }

    public DateTime Data { get; set; }
    public int Minuti { get; set; }
    public string? Nota { get; set; }
}
namespace ProjectTimeTracker.Api.Models;

public class ProgettoDettaglio
{
    public int Id { get; set; }

    public int ProgettoId { get; set; }
    public Progetto? Progetto { get; set; }

    public string Dettaglio { get; set; } = string.Empty;
    public DateTime Data { get; set; }

    public int UtenteId { get; set; }
    public Utente? Utente { get; set; }
}
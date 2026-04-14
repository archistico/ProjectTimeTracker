namespace ProjectTimeTracker.Api.Dtos;

public class ProgettoDettaglioDto
{
    public int Id { get; set; }
    public int ProgettoId { get; set; }
    public string Dettaglio { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public int UtenteId { get; set; }
    public string Utente { get; set; } = string.Empty;
}
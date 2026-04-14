namespace ProjectTimeTracker.Api.Models;

public class Utente
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;

    public ICollection<ProgettoDettaglio> ProgettoDettagli { get; set; } = new List<ProgettoDettaglio>();
    public ICollection<Cronologia> Cronologie { get; set; } = new List<Cronologia>();
    public ICollection<TempoLavorato> TempiLavorati { get; set; } = new List<TempoLavorato>();
}
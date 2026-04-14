namespace ProjectTimeTracker.Api.Models;

public class Progetto
{
    public int Id { get; set; }
    public string Oggetto { get; set; } = string.Empty;
    public string? Descrizione { get; set; }

    public int AreaId { get; set; }
    public Area? Area { get; set; }

    public int StatoId { get; set; }
    public Stato? Stato { get; set; }

    public int UrgenzaId { get; set; }
    public Urgenza? Urgenza { get; set; }

    public DateTime DataInizio { get; set; }
    public DateTime? DataFineRichiesta { get; set; }
    public DateTime? DataFineEffettiva { get; set; }

    public ICollection<ProgettoDettaglio> Dettagli { get; set; } = new List<ProgettoDettaglio>();
    public ICollection<Cronologia> Cronologie { get; set; } = new List<Cronologia>();
    public ICollection<TempoLavorato> TempiLavorati { get; set; } = new List<TempoLavorato>();
}
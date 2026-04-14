namespace ProjectTimeTracker.Api.Dtos;

public class ProgettoDto
{
    public int Id { get; set; }
    public string Oggetto { get; set; } = string.Empty;
    public string? Descrizione { get; set; }

    public int AreaId { get; set; }
    public string Area { get; set; } = string.Empty;

    public int StatoId { get; set; }
    public string Stato { get; set; } = string.Empty;

    public int UrgenzaId { get; set; }
    public string Urgenza { get; set; } = string.Empty;

    public DateTime DataInizio { get; set; }
    public DateTime? DataFineRichiesta { get; set; }
    public DateTime? DataFineEffettiva { get; set; }

    public int TotaleMinutiLavorati { get; set; }
}
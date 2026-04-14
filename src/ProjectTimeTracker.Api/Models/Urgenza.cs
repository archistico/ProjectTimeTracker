namespace ProjectTimeTracker.Api.Models;

public class Urgenza
{
    public int Id { get; set; }
    public string Descrizione { get; set; } = string.Empty;

    public ICollection<Progetto> Progetti { get; set; } = new List<Progetto>();
}
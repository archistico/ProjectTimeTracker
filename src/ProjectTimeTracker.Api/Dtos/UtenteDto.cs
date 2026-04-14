namespace ProjectTimeTracker.Api.Dtos;

public class UtenteDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Nominativo => $"{Cognome} {Nome}".Trim();
}
namespace ProjectTimeTracker.Api.Dtos;

public class UtenteCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
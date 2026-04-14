using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

public class TempoLavoratoCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ProgettoId deve essere maggiore di zero.")]
    public int ProgettoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UtenteId deve essere maggiore di zero.")]
    public int UtenteId { get; set; }

    public DateTime Data { get; set; }

    [Range(1, 1440)]
    public int Minuti { get; set; }

    [MaxLength(1000)]
    public string? Nota { get; set; }
}
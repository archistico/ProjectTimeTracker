using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

public class TempoLavoratoUpdateDto
{
    public DateTime Data { get; set; }

    [Range(1, 1440)]
    public int Minuti { get; set; }

    [MaxLength(1000)]
    public string? Nota { get; set; }
}
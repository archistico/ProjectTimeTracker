using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

public class CronologiaCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ProgettoId deve essere maggiore di zero.")]
    public int ProgettoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UtenteId deve essere maggiore di zero.")]
    public int UtenteId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Azione { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}
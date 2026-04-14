using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

public class ProgettoDettaglioCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ProgettoId deve essere maggiore di zero.")]
    public int ProgettoId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Dettaglio { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UtenteId deve essere maggiore di zero.")]
    public int UtenteId { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

public class ProgettoCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Oggetto { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descrizione { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "AreaId deve essere maggiore di zero.")]
    public int AreaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "StatoId deve essere maggiore di zero.")]
    public int StatoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UrgenzaId deve essere maggiore di zero.")]
    public int UrgenzaId { get; set; }

    public DateTime DataInizio { get; set; }
    public DateTime? DataFineRichiesta { get; set; }
}
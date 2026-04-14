using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Dtos;

/// <summary>
/// DTO usato per l'aggiornamento di un record di tempo lavorato.
/// A differenza della versione precedente, consente di modificare anche
/// progetto e utente associati al record.
/// </summary>
public class TempoLavoratoUpdateDto
{
    /// <summary>
    /// Identificativo del progetto associato.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "ProgettoId deve essere maggiore di zero.")]
    public int ProgettoId { get; set; }

    /// <summary>
    /// Identificativo dell'utente associato.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "UtenteId deve essere maggiore di zero.")]
    public int UtenteId { get; set; }

    /// <summary>
    /// Data dell'attività registrata.
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Minuti lavorati.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Minuti deve essere compreso tra 1 e 1440.")]
    public int Minuti { get; set; }

    /// <summary>
    /// Nota opzionale associata al tempo lavorato.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Nota non può superare i 1000 caratteri.")]
    public string? Nota { get; set; }
}
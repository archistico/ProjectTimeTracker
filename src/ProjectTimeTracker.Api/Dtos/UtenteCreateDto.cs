using System.ComponentModel.DataAnnotations;
using ProjectTimeTracker.Api.Validation;

namespace ProjectTimeTracker.Api.Dtos;

/// <summary>
/// DTO usato per la creazione di un nuovo utente.
/// </summary>
public class UtenteCreateDto
{
    /// <summary>
    /// Username univoco dell'utente.
    /// </summary>
    [Required(ErrorMessage = "Username è obbligatorio.")]
    [MaxLength(100, ErrorMessage = "Username non può superare i 100 caratteri.")]
    [NotWhiteSpace]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Cognome dell'utente.
    /// </summary>
    [Required(ErrorMessage = "Cognome è obbligatorio.")]
    [MaxLength(100, ErrorMessage = "Cognome non può superare i 100 caratteri.")]
    [NotWhiteSpace]
    public string Cognome { get; set; } = string.Empty;

    /// <summary>
    /// Nome dell'utente.
    /// </summary>
    [Required(ErrorMessage = "Nome è obbligatorio.")]
    [MaxLength(100, ErrorMessage = "Nome non può superare i 100 caratteri.")]
    [NotWhiteSpace]
    public string Nome { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Web.Models.Inputs;

public class SaveCurrentUserInput
{
    [Required(ErrorMessage = "Seleziona un utente.")]
    public int? UserId { get; set; }
}
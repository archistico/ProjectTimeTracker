using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Models.Inputs;
using ProjectTimeTracker.Web.Services;

namespace ProjectTimeTracker.Web.Pages.Sessione;

public class SelezionaUtenteModel : PageModel
{
    private readonly ILookupUiService _lookupUiService;
    private readonly IUserSessionService _userSessionService;

    public SelezionaUtenteModel(
        ILookupUiService lookupUiService,
        IUserSessionService userSessionService)
    {
        _lookupUiService = lookupUiService;
        _userSessionService = userSessionService;
    }

    [BindProperty]
    public SaveCurrentUserInput Input { get; set; } = new();

    public List<SelectListItem> Utenti { get; set; } = new();

    public string? LoadErrorMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadUtentiAsync(cancellationToken);

        var currentUser = _userSessionService.GetCurrentUser();
        if (currentUser != null)
        {
            Input.UserId = currentUser.Id;
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadUtentiAsync(cancellationToken);

        if (LoadErrorMessage != null)
        {
            ModelState.AddModelError(string.Empty, LoadErrorMessage);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var utenti = await TryGetUtentiAsync(cancellationToken);
        if (utenti == null)
        {
            ModelState.AddModelError(string.Empty, LoadErrorMessage ?? "Impossibile caricare gli utenti.");
            return Page();
        }

        var user = utenti.FirstOrDefault(x => x.Id == Input.UserId);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Utente non valido.");
            return Page();
        }

        _userSessionService.SetCurrentUser(user);
        return RedirectToPage("/Index");
    }

    private async Task LoadUtentiAsync(CancellationToken cancellationToken)
    {
        var utenti = await TryGetUtentiAsync(cancellationToken);

        if (utenti == null)
        {
            Utenti = new List<SelectListItem>();
            return;
        }

        Utenti = utenti
            .OrderBy(x => x.Cognome)
            .ThenBy(x => x.Nome)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Cognome} {x.Nome} ({x.Username})"
            })
            .ToList();
    }

    private async Task<List<ProjectTimeTracker.Web.Models.ViewModels.CurrentUserViewModel>?> TryGetUtentiAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            LoadErrorMessage = null;
            return await _lookupUiService.GetUtentiAsync(cancellationToken);
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.Message;
            return null;
        }
    }
}
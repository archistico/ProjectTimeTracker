using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTimeTracker.Web.Models.ViewModels;
using ProjectTimeTracker.Web.Services;

namespace ProjectTimeTracker.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IDashboardUiService _dashboardUiService;
    private readonly IUserSessionService _userSessionService;

    public IndexModel(
        IDashboardUiService dashboardUiService,
        IUserSessionService userSessionService)
    {
        _dashboardUiService = dashboardUiService;
        _userSessionService = userSessionService;
    }

    public DashboardViewModel Dashboard { get; set; } = new();

    public CurrentUserViewModel? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!_userSessionService.HasCurrentUser())
        {
            return RedirectToPage("/Sessione/SelezionaUtente");
        }

        CurrentUser = _userSessionService.GetCurrentUser();
        Dashboard = await _dashboardUiService.GetDashboardAsync(cancellationToken);

        return Page();
    }
}
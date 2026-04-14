using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public interface IDashboardUiService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
    string FormatMinutes(int minutes);
}
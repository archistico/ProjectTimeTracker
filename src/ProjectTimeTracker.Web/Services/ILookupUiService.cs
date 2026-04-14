using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public interface ILookupUiService
{
    Task<List<CurrentUserViewModel>> GetUtentiAsync(CancellationToken cancellationToken = default);
}
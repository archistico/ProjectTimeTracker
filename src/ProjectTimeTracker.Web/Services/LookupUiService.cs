using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public class LookupUiService : ILookupUiService
{
    private readonly IApiClient _apiClient;

    public LookupUiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CurrentUserViewModel>> GetUtentiAsync(CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetListAsync<CurrentUserViewModel>(ApiEndpoints.Utenti, cancellationToken);
    }
}
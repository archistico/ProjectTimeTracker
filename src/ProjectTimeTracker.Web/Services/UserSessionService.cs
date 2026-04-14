using System.Text.Json;
using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public class UserSessionService : IUserSessionService
{
    private const string SessionKey = "CurrentUser";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserViewModel? GetCurrentUser()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            return null;
        }

        var json = session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<CurrentUserViewModel>(json);
    }

    public void SetCurrentUser(CurrentUserViewModel user)
    {
        var session = _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Sessione non disponibile.");

        var json = JsonSerializer.Serialize(user);
        session.SetString(SessionKey, json);
    }

    public void ClearCurrentUser()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(SessionKey);
    }

    public bool HasCurrentUser()
    {
        return GetCurrentUser() != null;
    }
}
using ProjectTimeTracker.Web.Models.ViewModels;

namespace ProjectTimeTracker.Web.Services;

public interface IUserSessionService
{
    CurrentUserViewModel? GetCurrentUser();
    void SetCurrentUser(CurrentUserViewModel user);
    void ClearCurrentUser();
    bool HasCurrentUser();
}
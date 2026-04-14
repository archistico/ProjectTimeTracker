using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProjectTimeTracker.Api.Tests.Helpers;
using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Models.ViewModels;
using ProjectTimeTracker.Web.Services;

namespace ProjectTimeTracker.Api.Tests.Web;

public class LookupAndSessionTests
{
    [Fact]
    public async Task LookupUiService_Should_Return_Users_From_Api()
    {
        var apiClient = new FakeLookupApiClient();
        var service = new LookupUiService(apiClient);

        var result = await service.GetUtentiAsync();

        result.Should().HaveCount(2);
        result[0].Username.Should().Be("erollandin");
    }

    [Fact]
    public void UserSessionService_Should_Save_And_Read_Current_User()
    {
        var context = new DefaultHttpContext();
        context.Session = new TestSession();
        var accessor = new HttpContextAccessor { HttpContext = context };
        var service = new UserSessionService(accessor);

        service.SetCurrentUser(new CurrentUserViewModel
        {
            Id = 1,
            Username = "erollandin",
            Cognome = "Rollandin",
            Nome = "Emilie"
        });

        var user = service.GetCurrentUser();

        user.Should().NotBeNull();
        user!.DisplayName.Should().Be("Rollandin Emilie");
        service.HasCurrentUser().Should().BeTrue();
    }

    [Fact]
    public void UserSessionService_Clear_Should_Remove_Current_User()
    {
        var context = new DefaultHttpContext();
        context.Session = new TestSession();
        var accessor = new HttpContextAccessor { HttpContext = context };
        var service = new UserSessionService(accessor);

        service.SetCurrentUser(new CurrentUserViewModel { Id = 1, Username = "erollandin", Cognome = "Rollandin", Nome = "Emilie" });
        service.ClearCurrentUser();

        service.GetCurrentUser().Should().BeNull();
    }

    private sealed class FakeLookupApiClient : IApiClient
    {
        public Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default) => Task.FromResult(default(T));
        public Task<List<T>> GetListAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            var result = new List<CurrentUserViewModel>
            {
                new() { Id = 1, Username = "erollandin", Cognome = "Rollandin", Nome = "Emilie" },
                new() { Id = 2, Username = "mrossi", Cognome = "Rossi", Nome = "Mario" }
            };

            return Task.FromResult(result.Cast<T>().ToList());
        }
        public Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default) => Task.FromResult(default(TResponse));
        public Task<bool> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }
}

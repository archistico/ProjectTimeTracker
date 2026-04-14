using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTimeTracker.Api.Tests.Helpers;
using ProjectTimeTracker.Web.Models.Inputs;
using ProjectTimeTracker.Web.Models.ViewModels;
using ProjectTimeTracker.Web.Pages;
using ProjectTimeTracker.Web.Pages.Sessione;
using ProjectTimeTracker.Web.Services;

namespace ProjectTimeTracker.Api.Tests.Web;

public class PageModelsTests
{
    [Fact]
    public async Task IndexModel_Should_Redirect_To_User_Selection_When_No_Current_User()
    {
        var model = new IndexModel(new FakeDashboardUiService(), new FakeUserSessionService());

        var result = await model.OnGetAsync(CancellationToken.None);

        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result).PageName.Should().Be("/Sessione/SelezionaUtente");
    }

    [Fact]
    public async Task IndexModel_Should_Load_Dashboard_When_User_Is_Present()
    {
        var sessionService = new FakeUserSessionService
        {
            CurrentUser = new CurrentUserViewModel { Id = 1, Username = "erollandin", Cognome = "Rollandin", Nome = "Emilie" }
        };

        var model = new IndexModel(new FakeDashboardUiService(), sessionService);

        var result = await model.OnGetAsync(CancellationToken.None);

        result.Should().BeOfType<PageResult>();
        model.Dashboard.ProgettiAperti.Valore.Should().Be("5");
    }

    [Fact]
    public async Task SelezionaUtenteModel_OnGetAsync_Should_Load_Users_And_Set_Current_Selected_User()
    {
        var sessionService = new FakeUserSessionService
        {
            CurrentUser = new CurrentUserViewModel { Id = 2, Username = "mrossi", Cognome = "Rossi", Nome = "Mario" }
        };

        var model = new SelezionaUtenteModel(new FakeLookupUiService(), sessionService);

        await model.OnGetAsync(CancellationToken.None);

        model.Utenti.Should().HaveCount(2);
        model.Input.UserId.Should().Be(2);
    }

    [Fact]
    public async Task SelezionaUtenteModel_OnPostAsync_Should_Return_Page_When_ModelState_Is_Invalid()
    {
        var model = new SelezionaUtenteModel(new FakeLookupUiService(), new FakeUserSessionService())
        {
            Input = new SaveCurrentUserInput()
        };
        model.ModelState.AddModelError("Input.UserId", "Seleziona un utente.");

        var result = await model.OnPostAsync(CancellationToken.None);

        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task SelezionaUtenteModel_OnPostAsync_Should_Return_Page_When_User_Is_Not_Found()
    {
        var model = new SelezionaUtenteModel(new FakeLookupUiService(), new FakeUserSessionService())
        {
            Input = new SaveCurrentUserInput { UserId = 999 }
        };

        var result = await model.OnPostAsync(CancellationToken.None);

        result.Should().BeOfType<PageResult>();
        model.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task SelezionaUtenteModel_OnPostAsync_Should_Save_User_And_Redirect_When_Valid()
    {
        var sessionService = new FakeUserSessionService();
        var model = new SelezionaUtenteModel(new FakeLookupUiService(), sessionService)
        {
            Input = new SaveCurrentUserInput { UserId = 1 }
        };

        var result = await model.OnPostAsync(CancellationToken.None);

        result.Should().BeOfType<RedirectToPageResult>();
        sessionService.CurrentUser.Should().NotBeNull();
        sessionService.CurrentUser!.Id.Should().Be(1);
    }

    [Fact]
    public void SaveCurrentUserInput_Should_Require_UserId()
    {
        var model = new SaveCurrentUserInput();
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        isValid.Should().BeFalse();
        validationResults.Should().Contain(x => x.ErrorMessage == "Seleziona un utente.");
    }

    private sealed class FakeDashboardUiService : IDashboardUiService
    {
        public string FormatMinutes(int minutes) => $"{minutes}m";
        public Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new DashboardViewModel
            {
                ProgettiAperti = new DashboardStatCardViewModel { Valore = "5" }
            });
    }

    private sealed class FakeLookupUiService : ILookupUiService
    {
        public Task<List<CurrentUserViewModel>> GetUtentiAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CurrentUserViewModel>
            {
                new() { Id = 2, Username = "mrossi", Cognome = "Rossi", Nome = "Mario" },
                new() { Id = 1, Username = "erollandin", Cognome = "Rollandin", Nome = "Emilie" }
            });
    }

    private sealed class FakeUserSessionService : IUserSessionService
    {
        public CurrentUserViewModel? CurrentUser { get; set; }
        public void ClearCurrentUser() => CurrentUser = null;
        public CurrentUserViewModel? GetCurrentUser() => CurrentUser;
        public bool HasCurrentUser() => CurrentUser != null;
        public void SetCurrentUser(CurrentUserViewModel user) => CurrentUser = user;
    }
}

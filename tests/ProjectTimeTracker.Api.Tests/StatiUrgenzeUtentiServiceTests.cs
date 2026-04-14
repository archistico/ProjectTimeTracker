using FluentAssertions;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Services;
using ProjectTimeTracker.Api.Tests.Helpers;

namespace ProjectTimeTracker.Api.Tests;

public class StatiUrgenzeUtentiServiceTests
{
    [Fact]
    public async Task StatiService_Should_Return_All_Statuses()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new StatiService(db);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(5);
        result.First().Descrizione.Should().Be("Da iniziare");
    }

    [Fact]
    public async Task UrgenzeService_Should_Return_All_Urgencies()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new UrgenzeService(db);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Select(x => x.Descrizione).Should().Contain(new[] { "Non urgente", "Urgente" });
    }

    [Fact]
    public async Task UtentiService_GetByIdAsync_Should_Return_User_When_Found()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new UtentiService(db);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Username.Should().Be("erollandin");
    }

    [Fact]
    public async Task UtentiService_GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new UtentiService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UtentiService_CreateAsync_Should_Create_New_User()
    {
        using var db = TestDbFactory.CreateContext();
        var service = new UtentiService(db);

        var result = await service.CreateAsync(new UtenteCreateDto
        {
            Username = "gverdi",
            Cognome = "Verdi",
            Nome = "Giulia"
        });

        result.Id.Should().BeGreaterThan(0);
        result.Username.Should().Be("gverdi");
        db.Utenti.Should().Contain(x => x.Username == "gverdi");
    }
}

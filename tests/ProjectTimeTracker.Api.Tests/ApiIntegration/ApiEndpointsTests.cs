using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Tests.Helpers;

namespace ProjectTimeTracker.Api.Tests.ApiIntegration;

public class ApiEndpointsTests
{
    [Fact]
    public async Task Get_Utenti_Should_Return_Ok_And_List()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/utenti");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UtenteDto>>();
        users.Should().NotBeNull();
        users!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Get_Progetto_By_Id_Should_Return_NotFound_When_Missing()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/progetti/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Progetti_Should_Create_New_Project()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/progetti", new ProgettoCreateDto
        {
            Oggetto = "Nuovo progetto HTTP",
            Descrizione = "Creato via test integrazione",
            AreaId = 2,
            StatoId = 1,
            UrgenzaId = 2,
            DataInizio = DateTime.Today,
            DataFineRichiesta = DateTime.Today.AddDays(3)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ProgettoDto>();
        created.Should().NotBeNull();
        created!.Oggetto.Should().Be("Nuovo progetto HTTP");
    }

    [Fact]
    public async Task Put_Progetti_Should_Return_NoContent_When_Project_Exists()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/progetti/1", new ProgettoUpdateDto
        {
            Oggetto = "Progetto aggiornato HTTP",
            Descrizione = "Update test",
            AreaId = 1,
            StatoId = 2,
            UrgenzaId = 1,
            DataInizio = DateTime.Today,
            DataFineRichiesta = DateTime.Today.AddDays(10),
            DataFineEffettiva = null
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_TempoLavorato_Should_Return_NoContent_When_Entry_Exists()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/tempoLavorato/1");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Get_Dashboard_Should_Return_Aggregated_Data()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        dashboard.Should().NotBeNull();
        dashboard!.ProgettiAperti.Should().BeGreaterThan(0);
        dashboard.UltimiAggiornamenti.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Get_ProgettoTempoTotale_Should_Return_Total_Minutes()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/progetti/1/tempo/totale-minuti");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await response.Content.ReadFromJsonAsync<int>();
        total.Should().Be(90);
    }
}
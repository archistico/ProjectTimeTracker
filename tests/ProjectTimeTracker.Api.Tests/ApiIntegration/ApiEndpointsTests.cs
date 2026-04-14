using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Tests.Helpers;

namespace ProjectTimeTracker.Api.Tests.ApiIntegration;

/// <summary>
/// Contiene test di integrazione sugli endpoint principali dell'API.
/// </summary>
public class ApiEndpointsTests
{
    /// <summary>
    /// Verifica che l'endpoint utenti restituisca una lista con esito positivo.
    /// </summary>
    [Fact]
    public async Task Get_Utenti_Should_Return_Ok_And_List()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Utenti");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UtenteDto>>();
        users.Should().NotBeNull();
        users!.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifica che la richiesta di un progetto inesistente restituisca 404.
    /// </summary>
    [Fact]
    public async Task Get_Progetto_By_Id_Should_Return_NotFound_When_Missing()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Progetti/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Verifica che la creazione di un progetto restituisca 201 Created.
    /// </summary>
    [Fact]
    public async Task Post_Progetti_Should_Create_New_Project()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Progetti", new ProgettoCreateDto
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

    /// <summary>
    /// Verifica che l'aggiornamento di un progetto esistente restituisca 204 NoContent.
    /// </summary>
    [Fact]
    public async Task Put_Progetti_Should_Return_NoContent_When_Project_Exists()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/Progetti/100", new ProgettoUpdateDto
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

    /// <summary>
    /// Verifica che l'aggiornamento di un tempo lavorato possa cambiare anche progetto e utente.
    /// </summary>
    [Fact]
    public async Task Put_TempoLavorato_Should_Update_Project_And_User()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var updateResponse = await client.PutAsJsonAsync("/api/TempoLavorato/3000", new TempoLavoratoUpdateDto
        {
            ProgettoId = 101,
            UtenteId = 2,
            Data = DateTime.Today,
            Minuti = 150,
            Nota = "Tempo aggiornato con cambio riferimenti"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync("/api/TempoLavorato/3000");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await getResponse.Content.ReadFromJsonAsync<TempoLavoratoDto>();
        updated.Should().NotBeNull();
        updated!.ProgettoId.Should().Be(101);
        updated.UtenteId.Should().Be(2);
        updated.Minuti.Should().Be(150);
        updated.Nota.Should().Be("Tempo aggiornato con cambio riferimenti");
    }

    /// <summary>
    /// Verifica che la creazione di una voce di cronologia restituisca 201 Created.
    /// </summary>
    [Fact]
    public async Task Post_Cronologia_Should_Return_Created()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Cronologia", new CronologiaCreateDto
        {
            ProgettoId = 100,
            UtenteId = 1,
            Azione = "Test creazione cronologia",
            Data = DateTime.Today
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<CronologiaDto>();
        created.Should().NotBeNull();
        created!.Azione.Should().Be("Test creazione cronologia");
    }

    /// <summary>
    /// Verifica che la validazione del DTO utente blocchi campi vuoti o composti da spazi.
    /// </summary>
    [Fact]
    public async Task Post_Utenti_Should_Return_BadRequest_When_Required_Fields_Are_Invalid()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Utenti", new UtenteCreateDto
        {
            Username = "   ",
            Cognome = "",
            Nome = "   "
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Verifica che la cancellazione di un tempo lavorato esistente restituisca 204 NoContent.
    /// </summary>
    [Fact]
    public async Task Delete_TempoLavorato_Should_Return_NoContent_When_Entry_Exists()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/TempoLavorato/3000");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Verifica che la dashboard restituisca dati aggregati coerenti.
    /// </summary>
    [Fact]
    public async Task Get_Dashboard_Should_Return_Aggregated_Data()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        dashboard.Should().NotBeNull();
        dashboard!.ProgettiAperti.Should().BeGreaterThan(0);
        dashboard.UltimiAggiornamenti.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifica che il totale minuti di un progetto venga calcolato correttamente.
    /// </summary>
    [Fact]
    public async Task Get_ProgettoTempoTotale_Should_Return_Total_Minutes()
    {
        await using var factory = await TestApiFactory.CreateAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Progetti/100/tempo/totale-minuti");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var total = await response.Content.ReadFromJsonAsync<int>();
        total.Should().Be(135);
    }
}
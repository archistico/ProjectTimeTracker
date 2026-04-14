using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTimeTracker.Api.Controllers;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Middleware;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Tests.Helpers;

public sealed class TestApiFactory : IAsyncDisposable
{
    private WebApplication? _app;
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), "test_db.db");

    public static async Task<TestApiFactory> CreateAsync()
    {
        var factory = new TestApiFactory();
        await factory.InitializeAsync();
        return factory;
    }

    public HttpClient CreateClient()
    {
        if (_app == null)
        {
            throw new InvalidOperationException("L'host di test non è stato inizializzato.");
        }

        return _app.GetTestClient();
    }

    private async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        builder.WebHost.UseTestServer();

        builder.Services
            .AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={_dbPath}");
        });

        builder.Services.AddScoped<IUtentiService, UtentiService>();
        builder.Services.AddScoped<IAreeService, AreeService>();
        builder.Services.AddScoped<IStatiService, StatiService>();
        builder.Services.AddScoped<IUrgenzeService, UrgenzeService>();
        builder.Services.AddScoped<IProgettiService, ProgettiService>();
        builder.Services.AddScoped<IProgettoDettagliService, ProgettoDettagliService>();
        builder.Services.AddScoped<ICronologiaService, CronologiaService>();
        builder.Services.AddScoped<ITempoLavoratoService, TempoLavoratoService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("WebClient", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        _app = builder.Build();

        _app.UseRouting();
        _app.UseMiddleware<ExceptionHandlingMiddleware>();
        _app.UseCors("WebClient");
        _app.UseAuthorization();
        _app.MapControllers();

        await _app.StartAsync();

        using (var scope = _app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Delete and recreate to ensure clean state
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedTestData(db);
        }
    }

    private static void SeedTestData(AppDbContext db)
    {
        db.Utenti.AddRange(
            new Models.Utente
            {
                Id = 1,
                Username = "admin",
                Cognome = "Admin",
                Nome = "Sistema"
            },
            new Models.Utente
            {
                Id = 2,
                Username = "mrossi",
                Cognome = "Rossi",
                Nome = "Mario"
            });

        db.Aree.AddRange(
            new Models.Area { Id = 1, Descrizione = "Programmazione" },
            new Models.Area { Id = 2, Descrizione = "Grafica" });

        db.Stati.AddRange(
            new Models.Stato { Id = 1, Descrizione = "Da iniziare" },
            new Models.Stato { Id = 2, Descrizione = "In corso" },
            new Models.Stato { Id = 3, Descrizione = "Completato" });

        db.Urgenze.AddRange(
            new Models.Urgenza { Id = 1, Descrizione = "Non urgente" },
            new Models.Urgenza { Id = 2, Descrizione = "Urgente" });

        db.Progetti.AddRange(
            new Models.Progetto
            {
                Id = 1,
                Oggetto = "Progetto API Test",
                Descrizione = "Descrizione progetto API test",
                AreaId = 1,
                StatoId = 2,
                UrgenzaId = 1,
                DataInizio = new DateTime(2026, 4, 14),
                DataFineRichiesta = new DateTime(2026, 4, 20)
            },
            new Models.Progetto
            {
                Id = 2,
                Oggetto = "Progetto senza tempi",
                Descrizione = "Altro progetto",
                AreaId = 2,
                StatoId = 1,
                UrgenzaId = 2,
                DataInizio = new DateTime(2026, 4, 10)
            });

        db.TempiLavorati.AddRange(
            new Models.TempoLavorato
            {
                Id = 1,
                ProgettoId = 1,
                UtenteId = 1,
                Data = new DateTime(2026, 4, 14),
                Minuti = 60,
                Nota = "Analisi"
            },
            new Models.TempoLavorato
            {
                Id = 2,
                ProgettoId = 1,
                UtenteId = 2,
                Data = new DateTime(2026, 4, 15),
                Minuti = 30,
                Nota = "Implementazione"
            });

        db.Cronologie.Add(
            new Models.Cronologia
            {
                Id = 1,
                ProgettoId = 1,
                UtenteId = 1,
                Azione = "Creato progetto",
                Data = new DateTime(2026, 4, 14, 9, 0, 0)
            });

        db.ProgettoDettagli.Add(
            new Models.ProgettoDettaglio
            {
                Id = 1,
                ProgettoId = 1,
                UtenteId = 1,
                Dettaglio = "Dettaglio iniziale",
                Data = new DateTime(2026, 4, 14, 10, 0, 0)
            });

        db.SaveChanges();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
            _app = null;
        }

        // Clean up test database
        try
        {
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
        catch { }
    }
}
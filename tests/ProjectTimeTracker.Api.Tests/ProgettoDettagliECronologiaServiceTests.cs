using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Tests;

public class ProgettoDettagliECronologiaServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);

        db.Utenti.Add(new Utente
        {
            Id = 1,
            Username = "admin",
            Cognome = "Admin",
            Nome = "Sistema"
        });

        db.Aree.Add(new Area { Id = 1, Descrizione = "Programmazione" });
        db.Stati.Add(new Stato { Id = 1, Descrizione = "In corso" });
        db.Urgenze.Add(new Urgenza { Id = 1, Descrizione = "Urgente" });

        db.Progetti.Add(new Progetto
        {
            Id = 1,
            Oggetto = "Progetto 1",
            AreaId = 1,
            StatoId = 1,
            UrgenzaId = 1,
            DataInizio = new DateTime(2026, 4, 1)
        });

        db.ProgettoDettagli.Add(new ProgettoDettaglio
        {
            Id = 1,
            ProgettoId = 1,
            Dettaglio = "Dettaglio iniziale",
            Data = new DateTime(2026, 4, 2),
            UtenteId = 1
        });

        db.Cronologie.Add(new Cronologia
        {
            Id = 1,
            ProgettoId = 1,
            UtenteId = 1,
            Azione = "Creato progetto",
            Data = new DateTime(2026, 4, 1)
        });

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task ProgettoDettagliService_GetByProgettoIdAsync_Should_Return_Items()
    {
        using var db = CreateDbContext();
        var service = new ProgettoDettagliService(db);

        var result = await service.GetByProgettoIdAsync(1);

        result.Should().HaveCount(1);
        result[0].Dettaglio.Should().Be("Dettaglio iniziale");
    }

    [Fact]
    public async Task ProgettoDettagliService_CreateAsync_Should_Create_Item()
    {
        using var db = CreateDbContext();
        var service = new ProgettoDettagliService(db);

        var dto = new ProgettoDettaglioCreateDto
        {
            ProgettoId = 1,
            Dettaglio = "Nuovo dettaglio",
            Data = new DateTime(2026, 4, 3),
            UtenteId = 1
        };

        var result = await service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Dettaglio.Should().Be("Nuovo dettaglio");
        db.ProgettoDettagli.Count().Should().Be(2);
    }

    [Fact]
    public async Task ProgettoDettagliService_CreateAsync_Should_Throw_When_Project_Is_Invalid()
    {
        using var db = CreateDbContext();
        var service = new ProgettoDettagliService(db);

        var dto = new ProgettoDettaglioCreateDto
        {
            ProgettoId = 999,
            Dettaglio = "Nuovo dettaglio",
            Data = new DateTime(2026, 4, 3),
            UtenteId = 1
        };

        var act = async () => await service.CreateAsync(dto);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Progetto con chiave '999' non trovato.");
    }

    [Fact]
    public async Task CronologiaService_GetByProgettoIdAsync_Should_Return_Items()
    {
        using var db = CreateDbContext();
        var service = new CronologiaService(db);

        var result = await service.GetByProgettoIdAsync(1);

        result.Should().HaveCount(1);
        result[0].Azione.Should().Be("Creato progetto");
    }

    [Fact]
    public async Task CronologiaService_CreateAsync_Should_Create_Item()
    {
        using var db = CreateDbContext();
        var service = new CronologiaService(db);

        var dto = new CronologiaCreateDto
        {
            ProgettoId = 1,
            UtenteId = 1,
            Azione = "Aggiornato progetto",
            Data = new DateTime(2026, 4, 4)
        };

        var result = await service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Azione.Should().Be("Aggiornato progetto");
        db.Cronologie.Count().Should().Be(2);
    }

    [Fact]
    public async Task CronologiaService_CreateAsync_Should_Throw_When_User_Is_Invalid()
    {
        using var db = CreateDbContext();
        var service = new CronologiaService(db);

        var dto = new CronologiaCreateDto
        {
            ProgettoId = 1,
            UtenteId = 999,
            Azione = "Aggiornato progetto",
            Data = new DateTime(2026, 4, 4)
        };

        var act = async () => await service.CreateAsync(dto);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Utente con chiave '999' non trovato.");
    }
}
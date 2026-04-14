using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Tests;

public class ProgettiServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);

        db.Aree.AddRange(
            new Area { Id = 1, Descrizione = "Programmazione" },
            new Area { Id = 2, Descrizione = "Grafica" });

        db.Stati.AddRange(
            new Stato { Id = 1, Descrizione = "Da iniziare" },
            new Stato { Id = 2, Descrizione = "In corso" });

        db.Urgenze.AddRange(
            new Urgenza { Id = 1, Descrizione = "Non urgente" },
            new Urgenza { Id = 2, Descrizione = "Urgente" });

        db.Progetti.Add(new Progetto
        {
            Id = 1,
            Oggetto = "Progetto test",
            Descrizione = "Descrizione test",
            AreaId = 1,
            StatoId = 1,
            UrgenzaId = 1,
            DataInizio = new DateTime(2026, 4, 1)
        });

        db.TempiLavorati.AddRange(
            new TempoLavorato
            {
                Id = 1,
                ProgettoId = 1,
                UtenteId = 1,
                Data = new DateTime(2026, 4, 2),
                Minuti = 60,
                Nota = "Analisi"
            },
            new TempoLavorato
            {
                Id = 2,
                ProgettoId = 1,
                UtenteId = 1,
                Data = new DateTime(2026, 4, 3),
                Minuti = 30,
                Nota = "Sviluppo"
            });

        db.Utenti.Add(new Utente
        {
            Id = 1,
            Username = "admin",
            Cognome = "Admin",
            Nome = "Sistema"
        });

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Projects()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(1);
        result[0].Oggetto.Should().Be("Progetto test");
        result[0].TotaleMinutiLavorati.Should().Be(90);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Project_When_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.TotaleMinutiLavorati.Should().Be(90);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Project()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var dto = new ProgettoCreateDto
        {
            Oggetto = "Nuovo progetto",
            Descrizione = "Nuova descrizione",
            AreaId = 1,
            StatoId = 2,
            UrgenzaId = 2,
            DataInizio = new DateTime(2026, 4, 10),
            DataFineRichiesta = new DateTime(2026, 4, 20)
        };

        var result = await service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Oggetto.Should().Be("Nuovo progetto");
        db.Progetti.Count().Should().Be(2);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Area_Is_Invalid()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var dto = new ProgettoCreateDto
        {
            Oggetto = "Nuovo progetto",
            Descrizione = "Nuova descrizione",
            AreaId = 999,
            StatoId = 1,
            UrgenzaId = 1,
            DataInizio = new DateTime(2026, 4, 10)
        };

        var act = async () => await service.CreateAsync(dto);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Area con chiave '999' non trovato.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Project_When_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var dto = new ProgettoUpdateDto
        {
            Oggetto = "Progetto aggiornato",
            Descrizione = "Descrizione aggiornata",
            AreaId = 2,
            StatoId = 2,
            UrgenzaId = 2,
            DataInizio = new DateTime(2026, 4, 5),
            DataFineRichiesta = new DateTime(2026, 4, 25),
            DataFineEffettiva = new DateTime(2026, 4, 24)
        };

        var result = await service.UpdateAsync(1, dto);

        result.Should().BeTrue();

        var progetto = await db.Progetti.FindAsync(1);
        progetto.Should().NotBeNull();
        progetto!.Oggetto.Should().Be("Progetto aggiornato");
        progetto.AreaId.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Project_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var dto = new ProgettoUpdateDto
        {
            Oggetto = "Progetto aggiornato",
            Descrizione = "Descrizione aggiornata",
            AreaId = 1,
            StatoId = 1,
            UrgenzaId = 1,
            DataInizio = new DateTime(2026, 4, 5)
        };

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Project_When_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var result = await service.DeleteAsync(1);

        result.Should().BeTrue();
        db.Progetti.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Project_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new ProgettiService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }
}
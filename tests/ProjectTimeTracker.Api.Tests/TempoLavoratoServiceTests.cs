using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;
using ProjectTimeTracker.Api.Exceptions;
using ProjectTimeTracker.Api.Models;
using ProjectTimeTracker.Api.Services;

namespace ProjectTimeTracker.Api.Tests;

public class TempoLavoratoServiceTests
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

        db.Progetti.AddRange(
            new Progetto
            {
                Id = 1,
                Oggetto = "Progetto 1",
                AreaId = 1,
                StatoId = 1,
                UrgenzaId = 1,
                DataInizio = new DateTime(2026, 4, 1)
            },
            new Progetto
            {
                Id = 2,
                Oggetto = "Progetto 2",
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
                Data = new DateTime(2026, 4, 10),
                Minuti = 60,
                Nota = "Analisi"
            },
            new TempoLavorato
            {
                Id = 2,
                ProgettoId = 1,
                UtenteId = 1,
                Data = new DateTime(2026, 4, 11),
                Minuti = 30,
                Nota = "Sviluppo"
            });

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetTotaleMinutiByProgettoIdAsync_Should_Return_Total()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetTotaleMinutiByProgettoIdAsync(1);

        result.Should().Be(90);
    }

    [Fact]
    public async Task GetTotaleMinutiByProgettoIdAsync_Should_Return_Zero_When_No_Entries()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetTotaleMinutiByProgettoIdAsync(2);

        result.Should().Be(0);
    }

    [Fact]
    public async Task GetTotaleMinutiByProgettoIdAsync_Should_Throw_When_Project_Does_Not_Exist()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var act = async () => await service.GetTotaleMinutiByProgettoIdAsync(999);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Progetto con chiave '999' non trovato.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Record_When_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Minuti.Should().Be(60);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByProgettoIdAsync_Should_Return_Records()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetByProgettoIdAsync(1);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentiAsync_Should_Return_Requested_Number_Of_Records()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.GetRecentiAsync(1);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Record()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var dto = new TempoLavoratoCreateDto
        {
            ProgettoId = 1,
            UtenteId = 1,
            Data = new DateTime(2026, 4, 12),
            Minuti = 45,
            Nota = "Test"
        };

        var result = await service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Minuti.Should().Be(45);
        db.TempiLavorati.Count().Should().Be(3);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Project_Is_Invalid()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var dto = new TempoLavoratoCreateDto
        {
            ProgettoId = 999,
            UtenteId = 1,
            Data = new DateTime(2026, 4, 12),
            Minuti = 45,
            Nota = "Test"
        };

        var act = async () => await service.CreateAsync(dto);

        await act.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Progetto con chiave '999' non trovato.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Record_When_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var dto = new TempoLavoratoUpdateDto
        {
            Data = new DateTime(2026, 4, 15),
            Minuti = 120,
            Nota = "Aggiornato"
        };

        var result = await service.UpdateAsync(1, dto);

        result.Should().BeTrue();

        var entity = await db.TempiLavorati.FindAsync(1);
        entity.Should().NotBeNull();
        entity!.Minuti.Should().Be(120);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var dto = new TempoLavoratoUpdateDto
        {
            Data = new DateTime(2026, 4, 15),
            Minuti = 120,
            Nota = "Aggiornato"
        };

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Record_When_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.DeleteAsync(1);

        result.Should().BeTrue();
        db.TempiLavorati.Count().Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Not_Found()
    {
        using var db = CreateDbContext();
        var service = new TempoLavoratoService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }
}
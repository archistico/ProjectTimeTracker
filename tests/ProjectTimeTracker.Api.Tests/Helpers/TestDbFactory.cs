using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Tests.Helpers;

internal static class TestDbFactory
{
    public static AppDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        SeedBaseData(db);
        return db;
    }

    public static void SeedBaseData(AppDbContext db)
    {
        if (db.Aree.Any())
        {
            return;
        }

        db.Aree.AddRange(
            new Area { Id = 1, Descrizione = "Grafica" },
            new Area { Id = 2, Descrizione = "Programmazione" },
            new Area { Id = 3, Descrizione = "Casa Editrice" });

        db.Stati.AddRange(
            new Stato { Id = 1, Descrizione = "Da iniziare" },
            new Stato { Id = 2, Descrizione = "In corso" },
            new Stato { Id = 3, Descrizione = "Sospeso" },
            new Stato { Id = 4, Descrizione = "Annullato" },
            new Stato { Id = 5, Descrizione = "Completato" });

        db.Urgenze.AddRange(
            new Urgenza { Id = 1, Descrizione = "Non urgente" },
            new Urgenza { Id = 2, Descrizione = "Urgente" });

        db.Utenti.AddRange(
            new Utente { Id = 1, Username = "erollandin", Cognome = "Rollandin", Nome = "Emilie" },
            new Utente { Id = 2, Username = "mrossi", Cognome = "Rossi", Nome = "Mario" },
            new Utente { Id = 3, Username = "lbianchi", Cognome = "Bianchi", Nome = "Laura" });

        db.Progetti.AddRange(
            new Progetto
            {
                Id = 100,
                Oggetto = "Sito casa editrice",
                Descrizione = "Restyling frontend",
                AreaId = 1,
                StatoId = 2,
                UrgenzaId = 2,
                DataInizio = new DateTime(2026, 4, 1),
                DataFineRichiesta = DateTime.Today.AddDays(-1)
            },
            new Progetto
            {
                Id = 101,
                Oggetto = "API ProjectTimeTracker",
                Descrizione = "Backend .NET 8",
                AreaId = 2,
                StatoId = 2,
                UrgenzaId = 1,
                DataInizio = new DateTime(2026, 4, 10),
                DataFineRichiesta = DateTime.Today.AddDays(5)
            },
            new Progetto
            {
                Id = 102,
                Oggetto = "Libro inglese",
                Descrizione = "Impaginazione finale",
                AreaId = 3,
                StatoId = 5,
                UrgenzaId = 1,
                DataInizio = new DateTime(2026, 3, 1),
                DataFineRichiesta = DateTime.Today.AddDays(-10),
                DataFineEffettiva = DateTime.Today.AddDays(-2)
            });

        db.ProgettoDettagli.AddRange(
            new ProgettoDettaglio
            {
                Id = 1000,
                ProgettoId = 100,
                UtenteId = 1,
                Dettaglio = "Creata bozza wireframe",
                Data = DateTime.Today.AddDays(-2)
            },
            new ProgettoDettaglio
            {
                Id = 1001,
                ProgettoId = 100,
                UtenteId = 2,
                Dettaglio = "Aggiornata palette colori",
                Data = DateTime.Today.AddDays(-1)
            });

        db.Cronologie.AddRange(
            new Cronologia
            {
                Id = 2000,
                ProgettoId = 100,
                UtenteId = 1,
                Azione = "Creato progetto",
                Data = DateTime.Today.AddDays(-3)
            },
            new Cronologia
            {
                Id = 2001,
                ProgettoId = 101,
                UtenteId = 2,
                Azione = "Registrato tempo sviluppo",
                Data = DateTime.Today.AddHours(-2)
            },
            new Cronologia
            {
                Id = 2002,
                ProgettoId = 100,
                UtenteId = 3,
                Azione = "Aggiornato stato",
                Data = DateTime.Today.AddHours(-1)
            });

        db.TempiLavorati.AddRange(
            new TempoLavorato
            {
                Id = 3000,
                ProgettoId = 100,
                UtenteId = 1,
                Data = DateTime.Today,
                Minuti = 90,
                Nota = "Analisi iniziale"
            },
            new TempoLavorato
            {
                Id = 3001,
                ProgettoId = 100,
                UtenteId = 2,
                Data = DateTime.Today.AddDays(-1),
                Minuti = 45,
                Nota = "Correzioni UI"
            },
            new TempoLavorato
            {
                Id = 3002,
                ProgettoId = 101,
                UtenteId = 2,
                Data = DateTime.Today.AddDays(-6),
                Minuti = 120,
                Nota = "Endpoint API"
            },
            new TempoLavorato
            {
                Id = 3003,
                ProgettoId = 102,
                UtenteId = 3,
                Data = DateTime.Today.AddDays(-20),
                Minuti = 30,
                Nota = "Revisione finale"
            });

        db.SaveChanges();
    }
}

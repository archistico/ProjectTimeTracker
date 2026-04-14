using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.Aree.Any())
        {
            db.Aree.AddRange(
                new Area { Descrizione = "Grafica" },
                new Area { Descrizione = "Programmazione" },
                new Area { Descrizione = "Casa Editrice" }
            );
        }

        if (!db.Stati.Any())
        {
            db.Stati.AddRange(
                new Stato { Descrizione = "Da iniziare" },
                new Stato { Descrizione = "In corso" },
                new Stato { Descrizione = "Sospeso" },
                new Stato { Descrizione = "Annullato" },
                new Stato { Descrizione = "Completato" }
            );
        }

        if (!db.Urgenze.Any())
        {
            db.Urgenze.AddRange(
                new Urgenza { Descrizione = "Non urgente" },
                new Urgenza { Descrizione = "Urgente" }
            );
        }

        if (!db.Utenti.Any())
        {
            db.Utenti.Add(new Utente
            {
                Username = "admin",
                Cognome = "Admin",
                Nome = "Sistema"
            });
        }

        db.SaveChanges();
    }
}
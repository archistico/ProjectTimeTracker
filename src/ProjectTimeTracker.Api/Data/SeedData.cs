using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Data;

/// <summary>
/// Gestisce l'inizializzazione dei dati minimi dell'applicazione.
/// Il seed è idempotente: ogni gruppo di dati viene verificato e inserito
/// in modo indipendente all'interno di una transazione.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Esegue il seed dei dati di base se mancanti.
    /// </summary>
    /// <param name="db">Contesto applicativo.</param>
    public static void Initialize(AppDbContext db)
    {
        using var transaction = db.Database.BeginTransaction();

        SeedAree(db);
        db.SaveChanges();

        SeedStati(db);
        db.SaveChanges();

        SeedUrgenze(db);
        db.SaveChanges();

        SeedUtenti(db);
        db.SaveChanges();

        transaction.Commit();
    }

    /// <summary>
    /// Inserisce le aree mancanti.
    /// </summary>
    private static void SeedAree(AppDbContext db)
    {
        EnsureArea(db, "Grafica");
        EnsureArea(db, "Programmazione");
        EnsureArea(db, "Casa Editrice");
    }

    /// <summary>
    /// Inserisce gli stati mancanti.
    /// </summary>
    private static void SeedStati(AppDbContext db)
    {
        EnsureStato(db, "Da iniziare");
        EnsureStato(db, "In corso");
        EnsureStato(db, "Sospeso");
        EnsureStato(db, "Annullato");
        EnsureStato(db, "Completato");
    }

    /// <summary>
    /// Inserisce le urgenze mancanti.
    /// </summary>
    private static void SeedUrgenze(AppDbContext db)
    {
        EnsureUrgenza(db, "Non urgente");
        EnsureUrgenza(db, "Urgente");
    }

    /// <summary>
    /// Inserisce gli utenti base mancanti.
    /// </summary>
    private static void SeedUtenti(AppDbContext db)
    {
        if (!db.Utenti.Any(x => x.Username == "admin"))
        {
            db.Utenti.Add(new Utente
            {
                Username = "admin",
                Cognome = "Admin",
                Nome = "Sistema"
            });
        }
    }

    /// <summary>
    /// Garantisce la presenza di un'area.
    /// </summary>
    private static void EnsureArea(AppDbContext db, string descrizione)
    {
        if (!db.Aree.Any(x => x.Descrizione == descrizione))
        {
            db.Aree.Add(new Area { Descrizione = descrizione });
        }
    }

    /// <summary>
    /// Garantisce la presenza di uno stato.
    /// </summary>
    private static void EnsureStato(AppDbContext db, string descrizione)
    {
        if (!db.Stati.Any(x => x.Descrizione == descrizione))
        {
            db.Stati.Add(new Stato { Descrizione = descrizione });
        }
    }

    /// <summary>
    /// Garantisce la presenza di un'urgenza.
    /// </summary>
    private static void EnsureUrgenza(AppDbContext db, string descrizione)
    {
        if (!db.Urgenze.Any(x => x.Descrizione == descrizione))
        {
            db.Urgenze.Add(new Urgenza { Descrizione = descrizione });
        }
    }
}
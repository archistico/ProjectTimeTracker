using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Data;

/// <summary>
/// Espone il contratto minimo del contesto applicativo usato dai service.
/// Permette di disaccoppiare i service dall'implementazione concreta di <see cref="AppDbContext"/>.
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    /// Ottiene il set degli utenti.
    /// </summary>
    DbSet<Utente> Utenti { get; }

    /// <summary>
    /// Ottiene il set delle aree.
    /// </summary>
    DbSet<Area> Aree { get; }

    /// <summary>
    /// Ottiene il set degli stati.
    /// </summary>
    DbSet<Stato> Stati { get; }

    /// <summary>
    /// Ottiene il set delle urgenze.
    /// </summary>
    DbSet<Urgenza> Urgenze { get; }

    /// <summary>
    /// Ottiene il set dei progetti.
    /// </summary>
    DbSet<Progetto> Progetti { get; }

    /// <summary>
    /// Ottiene il set dei dettagli progetto.
    /// </summary>
    DbSet<ProgettoDettaglio> ProgettoDettagli { get; }

    /// <summary>
    /// Ottiene il set della cronologia.
    /// </summary>
    DbSet<Cronologia> Cronologie { get; }

    /// <summary>
    /// Ottiene il set dei tempi lavorati.
    /// </summary>
    DbSet<TempoLavorato> TempiLavorati { get; }

    /// <summary>
    /// Salva le modifiche pendenti nel database.
    /// </summary>
    /// <param name="cancellationToken">Token di annullamento.</param>
    /// <returns>Numero di entità salvate.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
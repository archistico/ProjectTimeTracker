using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

/// <summary>
/// Definisce le operazioni applicative disponibili per la cronologia.
/// </summary>
public interface ICronologiaService
{
    /// <summary>
    /// Restituisce una voce di cronologia per identificativo.
    /// </summary>
    /// <param name="id">Identificativo della voce.</param>
    Task<CronologiaDto?> GetByIdAsync(int id);

    /// <summary>
    /// Restituisce la cronologia associata a un progetto.
    /// </summary>
    /// <param name="progettoId">Identificativo del progetto.</param>
    Task<List<CronologiaDto>> GetByProgettoIdAsync(int progettoId);

    /// <summary>
    /// Crea una nuova voce di cronologia.
    /// </summary>
    /// <param name="dto">Dati della voce da creare.</param>
    Task<CronologiaDto> CreateAsync(CronologiaCreateDto dto);
}
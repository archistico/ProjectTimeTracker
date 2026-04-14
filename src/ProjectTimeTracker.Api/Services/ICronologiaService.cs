using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface ICronologiaService
{
    Task<List<CronologiaDto>> GetByProgettoIdAsync(int progettoId);
    Task<CronologiaDto> CreateAsync(CronologiaCreateDto dto);
}
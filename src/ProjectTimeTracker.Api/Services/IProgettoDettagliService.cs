using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IProgettoDettagliService
{
    Task<List<ProgettoDettaglioDto>> GetByProgettoIdAsync(int progettoId);
    Task<ProgettoDettaglioDto> CreateAsync(ProgettoDettaglioCreateDto dto);
}
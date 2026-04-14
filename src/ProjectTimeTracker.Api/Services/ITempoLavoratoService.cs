using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface ITempoLavoratoService
{
    Task<List<TempoLavoratoDto>> GetByProgettoIdAsync(int progettoId);
    Task<List<TempoLavoratoDto>> GetRecentiAsync(int take);
    Task<int> GetTotaleMinutiByProgettoIdAsync(int progettoId);
    Task<TempoLavoratoDto?> GetByIdAsync(int id);
    Task<TempoLavoratoDto> CreateAsync(TempoLavoratoCreateDto dto);
    Task<bool> UpdateAsync(int id, TempoLavoratoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
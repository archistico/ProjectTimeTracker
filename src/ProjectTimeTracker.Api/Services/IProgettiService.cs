using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IProgettiService
{
    Task<List<ProgettoDto>> GetAllAsync();
    Task<ProgettoDto?> GetByIdAsync(int id);
    Task<ProgettoDto> CreateAsync(ProgettoCreateDto dto);
    Task<bool> UpdateAsync(int id, ProgettoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
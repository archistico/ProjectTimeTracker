using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IUtentiService
{
    Task<List<UtenteDto>> GetAllAsync();
    Task<UtenteDto?> GetByIdAsync(int id);
    Task<UtenteDto> CreateAsync(UtenteCreateDto dto);
}
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IUrgenzeService
{
    Task<List<UrgenzaDto>> GetAllAsync();
}
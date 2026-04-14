using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IAreeService
{
    Task<List<AreaDto>> GetAllAsync();
}
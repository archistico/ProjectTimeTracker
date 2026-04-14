using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IStatiService
{
    Task<List<StatoDto>> GetAllAsync();
}
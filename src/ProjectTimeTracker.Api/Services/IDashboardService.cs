using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
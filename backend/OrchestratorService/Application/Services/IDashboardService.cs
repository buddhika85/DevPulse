using OrchestratorService.Application.DTOs;

namespace OrchestratorService.Application.Services
{
    public interface IDashboardService
    {
        // GetDashboardAsync is legacy will be removed soon
        Task<DashboardDto> GetDashboardAsync(string userId, CancellationToken cancellationToken);

        // GetDashboardAsync new version
        Task<BaseDashboardDto> GetUserDashboardAsync(string userId, CancellationToken cancellationToken);



        void InvalidateDashboardCache(string userId);
    }
}
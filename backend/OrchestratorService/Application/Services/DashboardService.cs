using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients;

namespace OrchestratorService.Application.Services
{
    public class DashboardService
    {
        private readonly IUserServiceClient _userClient;
        private readonly ITaskServiceClient _taskClient;
        private readonly IMemoryCache _inMemoryCache;

        private const byte CachedTimeInMins = 5;                // Cache time in minutes


        public DashboardService(IUserServiceClient userClient, ITaskServiceClient taskClient, IMemoryCache cache)
        {
            _userClient = userClient;
            _taskClient = taskClient;
            _inMemoryCache = cache;
        }

        /// <summary>
        /// Composes dashboard data from multiple services.
        /// Uses in-memory caching to avoid redundant downstream API calls.
        /// </summary>
        public async Task<DashboardDto> GetDashboardAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid user ID format");

            var cacheKey = $"dashboard:{userGuid}";

            // Check in-memory cache first
            if (_inMemoryCache.TryGetValue(cacheKey, out DashboardDto? cachedDashboard) && cachedDashboard is not null)
                return cachedDashboard;

            // No cache found - so call APIs
            // Fetch from downstream services
            var user = await _userClient.GetUserAsync(userId);
            var tasks = await _taskClient.GetTasksAsync(userId);

            var dashboard = new DashboardDto { User = user, Tasks = tasks };

            // Cache the composed result for n minutes
            _inMemoryCache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(CachedTimeInMins));

            return dashboard;
        }

    }
}

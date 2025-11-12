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
        private readonly ILogger<DashboardService> _logger;
        private const byte CachedTimeInMins = 5;                // Cache time in minutes


        public DashboardService(IUserServiceClient userClient, ITaskServiceClient taskClient, IMemoryCache cache, ILogger<DashboardService> logger)
        {
            _userClient = userClient;
            _taskClient = taskClient;
            _inMemoryCache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Composes dashboard data from multiple services.
        /// Uses in-memory caching to avoid redundant downstream API calls.
        /// </summary>
        public async Task<DashboardDto> GetDashboardAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            var cacheKey = $"dashboard:{userGuid}";
            try
            {
                _logger.LogInformation("Attempting to retrieve dashboard for user {UserId} from cache", userGuid);

                // Check in-memory cache first
                if (_inMemoryCache.TryGetValue(cacheKey, out DashboardDto? cachedDashboard) && cachedDashboard is not null)
                {
                    _logger.LogInformation("Dashboard cache hit for user {UserId}", userGuid);
                    return cachedDashboard;
                }

                _logger.LogInformation("Dashboard cache miss/unavailable for user {UserId}. Fetching from downstream services...", userGuid);


                // No cache found - so call APIs
                // Fetch from downstream services
                var user = await _userClient.GetUserAsync(userId, cancellationToken);
                var tasks = await _taskClient.GetTasksAsync(userId, cancellationToken);

                var dashboard = new DashboardDto { User = user, Tasks = tasks };

                // Cache the composed result for n minutes
                _inMemoryCache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(CachedTimeInMins));
                _logger.LogInformation("Dashboard cached for user {UserId} with expiration of {CacheDuration} minutes", userGuid, CachedTimeInMins);


                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve dashboard for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard retrieval failed", ex);
            }
        }

        /// <summary>
        /// Invalidates the dashboard cache for a specific user if it exists.
        /// Ensures the 
        public void InvalidateDashboardCache(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            var cacheKey = $"dashboard:{userGuid}";

            try
            {
                _logger.LogInformation("Attempting to invalidate dashboard cache for user {UserId}", userGuid);

                // Check in-memory cache first
                if (!_inMemoryCache.TryGetValue(cacheKey, out DashboardDto? cachedDashboard) || cachedDashboard is null)
                {
                    _logger.LogInformation("No dashboard cache found for user {UserId}. Nothing to invalidate.", userGuid);
                    return;         // no cache to remove
                }

                _inMemoryCache.Remove(cacheKey); 
                _logger.LogInformation("Dashboard cache successfully invalidated for user {UserId}", userGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while invalidating dashboard cache for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard cache invalidation failed", ex);
            }

        }
    }
}

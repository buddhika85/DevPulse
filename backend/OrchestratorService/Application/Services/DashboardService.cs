using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using SharedLib.DTOs.User;

namespace OrchestratorService.Application.Services
{
    public class DashboardService : IDashboardService
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
        /// Legacy
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
                var tasks = await _taskClient.GetTasksAsync(userId, cancellationToken) ?? [];

                var dashboard = new DashboardDto(user, tasks);

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
        /// Returns a hydrated dto of Dashboard for the userId based on user role
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken for cancelling</param>
        /// <returns>BaseDashboardDto subtype</returns>
        public async Task<BaseDashboardDto> GetUserDashboardAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }
            try
            {
                _logger.LogInformation("Attempting to retrieve dashboard for user {UserId} from cache", userGuid);
                var user = await _userClient.GetUserAsync(userId, cancellationToken);

                if (user is null)
                {
                    _logger.LogError("No user with Id {UserId}. Unable to construct a dashboard result.", userGuid);
                    throw new ArgumentException($"Dashboard retrieval failed as there is no user with Id {userGuid}");
                }

                // return appropriate sub type of BaseDashboardDto based on user role
                return ConstructuserDashboard(userGuid, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve dashboard for user {UserId}", userGuid);
                throw new ApplicationException("Dashboard retrieval failed", ex);
            }
        }

        private BaseDashboardDto ConstructuserDashboard(Guid userGuid, UserAccountDto user)
        {
            switch (user.UserRole.ToLower())
            {
                case "admin":
                    {
                        _logger.LogInformation("Constructing Admin dashboard for {UserId}", userGuid);

                        // To Do: more admin specific dashboard data
                        // ..

                        return new AdminDashboardDto(user);
                    }
                case "user":
                    {
                        _logger.LogInformation("Constructing User dashboard for {UserId}", userGuid);

                        // To Do: more user (developer) specific dashboard data
                        // ..

                        return new UserDashboardDto(user);
                    }
                case "manager":
                    {
                        _logger.LogInformation("Constructing Manager dashboard for {UserId}", userGuid);

                        // To Do: more manager specific dashboard data
                        // ..


                        return new ManagerDashboardDto(user);
                    }
                default:
                    throw new ApplicationException($"Invalid user role for user {userGuid}");
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

using SharedLib.DTOs.User;
using System.Text.Json;


namespace OrchestratorService.Infrastructure.HttpClients.UserMicroService
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;

        private const string routeToProfileController = "api/profile";
        private const string routeToUserController = "api/users";

        public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserAccountDto> GetUserAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                _logger.LogWarning("Invalid user ID format received: {UserId}", userId);
                throw new ArgumentException("Invalid user ID format", nameof(userId));
            }

            try
            {
                _logger.LogInformation("Fetching user profile for user ID: {UserId}", userIdGuid);

                var user = await _httpClient.GetFromJsonAsync<UserAccountDto>(
                    $"{routeToProfileController}/{userIdGuid}", cancellationToken);                                    // api/profile/user-id

                if (user is null)
                {
                    _logger.LogWarning("User profile not found for user ID: {UserId}", userIdGuid);
                    throw new InvalidOperationException("User not found");
                }

                _logger.LogInformation("Successfully retrieved user profile for user ID: {UserId}", userIdGuid);
                return user;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching user profile for user ID: {UserId}", userIdGuid);
                throw new ApplicationException("Failed to fetch user profile from API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for user profile response for user ID: {UserId}", userIdGuid);
                throw new ApplicationException("Failed to deserialize user profile data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("User profile fetch operation was canceled for user ID: {UserId}", userIdGuid);
                throw;
            }
        }

        public async Task<IReadOnlyList<Guid>> GetTeamMembersIdsForManager(Guid managerId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Fetching team user IDs for manager {ManagerId} at {Time}", managerId, now);

            try
            {
                var url = $"{routeToUserController}/team-for-manager";

                _logger.LogInformation("Calling User API endpoint {Url} for manager {ManagerId}", url, managerId);

                var response = await _httpClient.GetAsync(url, cancellationToken);

                // Throws if status code is not 2xx
                response.EnsureSuccessStatusCode();

                var userIds = await response.Content.ReadFromJsonAsync<IReadOnlyList<Guid>>(cancellationToken: cancellationToken);

                if (userIds is null)
                {
                    _logger.LogWarning("User API returned null team member list for manager {ManagerId}", managerId);
                    throw new InvalidOperationException($"Team user IDs not found for manager {managerId}");
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} team member IDs for manager {ManagerId} at {Time}",
                    userIds.Count, managerId, now);

                return userIds;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching team user IDs for manager {ManagerId}", managerId);
                throw new ApplicationException("Failed to fetch team user IDs from User API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for team user IDs for manager {ManagerId}", managerId);
                throw new ApplicationException("Failed to deserialize team user ID data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Team user ID fetch operation was canceled for manager {ManagerId}", managerId);
                throw;
            }
        }


        public async Task<IReadOnlyList<UserAccountDto>> GetTeamMembersForManager(Guid managerId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Fetching team user users for manager {ManagerId} at {Time}", managerId, now);

            try
            {
                var url = $"{routeToProfileController}/team-for-manager";

                _logger.LogInformation("Calling User API endpoint {Url} for manager {ManagerId}", url, managerId);

                var response = await _httpClient.GetAsync(url, cancellationToken);

                // Throws if status code is not 2xx
                response.EnsureSuccessStatusCode();

                var userIds = await response.Content.ReadFromJsonAsync<IReadOnlyList<UserAccountDto>>(cancellationToken: cancellationToken);

                if (userIds is null)
                {
                    _logger.LogWarning("User API returned null team member list for manager {ManagerId}", managerId);
                    throw new InvalidOperationException($"Team user users not found for manager {managerId}");
                }

                _logger.LogInformation(
                    "Successfully retrieved {Count} team member users for manager {ManagerId} at {Time}",
                    userIds.Count, managerId, now);

                return userIds;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching team users for manager {ManagerId}", managerId);
                throw new ApplicationException("Failed to fetch team user from User API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for team users for manager {ManagerId}", managerId);
                throw new ApplicationException("Failed to deserialize team user data", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Team members fetch operation was canceled for manager {ManagerId}", managerId);
                throw;
            }
        }

    }

}

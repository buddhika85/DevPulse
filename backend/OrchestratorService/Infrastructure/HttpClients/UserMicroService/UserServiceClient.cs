using SharedLib.DTOs.User;
using System.Text.Json;


namespace OrchestratorService.Infrastructure.HttpClients.UserMicroService
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;

        private const string routeUserById = "api/profile/";

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
                    $"{routeUserById}{userIdGuid}", cancellationToken);                                    // api/profile/user-id

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
    }

}

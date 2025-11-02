using System.Text.Json;
using UserService.Application.Dtos;

namespace UserService.Infrastructure.Identity
{
    public class EntraIdentityProvider : IExternalIdentityProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EntraIdentityProvider> _logger;

        public EntraIdentityProvider(HttpClient httpClient, ILogger<EntraIdentityProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }


       
        public async Task<UserAccountDto?> GetUserByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            // 🔐 Construct the Microsoft Graph URL using the user's Object ID
            // 🔐 Replace with actual call to Microsoft Graph or Entra External ID
            var url = $"https://graph.microsoft.com/v1.0/users/{objectId}";

            try
            {
                // 🌐 Make an HTTP GET request to Microsoft Graph
                var response = await _httpClient.GetAsync(url, cancellationToken);

                // ❌ If the response is not successful (e.g., 404, 401), log and return null
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch user from Entra. Status: {Status}", response.StatusCode);
                    return null;
                }

                // 📦 Read the response body as JSON
                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                // 🔄 Deserialize the JSON into your DTO, ignoring case differences
                var dto = JsonSerializer.Deserialize<UserAccountDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // ✅ Return the user profile
                return dto;
            }
            catch (Exception ex)
            {
                // 🧨 Log any unexpected errors and return null
                _logger.LogError(ex, "Error fetching user from Entra for ObjectId: {ObjectId}", objectId);
                return null;
            }
        }
    }
}

using SharedLib.DTOs.User;
using System.Net.Http.Headers;
using System.Text.Json;
using UserService.Application.Common.Mappers;

namespace UserService.Infrastructure.Identity
{
    public class EntraIdentityProvider : IExternalIdentityProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EntraIdentityProvider> _logger;
        private readonly GraphTokenService _graphTokenService;

        public EntraIdentityProvider(HttpClient httpClient, ILogger<EntraIdentityProvider> logger, GraphTokenService graphTokenService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _graphTokenService = graphTokenService;
        }

        public async Task<UserAccountDto?> GetUserByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            // 🔐 Construct the Microsoft Graph URL using the user's Object ID
            var url = $"https://graph.microsoft.com/v1.0/users/{objectId}";

            try
            {
                // 🧪 Step 1: Request token from Microsoft Identity Platform
                var token = await _graphTokenService.GetAccessTokenAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Failed to acquire Graph token from GraphTokenService.");
                    return null;
                }

                // 🧵 Step 2: Attach token to HttpClient for Graph call
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 🌐 Step 3: Call Microsoft Graph to fetch user profile
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch user from Graph. Status: {Status}", response.StatusCode);
                    return null;
                }

                // 🔄 Step 4: Deserialize response into DTO
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                //var dto = JsonSerializer.Deserialize<UserAccountDto>(json, new JsonSerializerOptions
                //{
                //    PropertyNameCaseInsensitive = true
                //});

                // Parse the JSON string into a JsonDocument
                using var document = JsonDocument.Parse(json);

                var dto = UserAccountMapper.FromGraphUser(document.RootElement);

                // ✅ Step 5: Return the user profile
                return dto;
            }
            catch (Exception ex)
            {
                // 🧨 Log any unexpected errors and return null
                _logger.LogError(ex, "Error fetching user from Graph for ObjectId: {ObjectId}", objectId);
                return null;
            }
        }
    }
}

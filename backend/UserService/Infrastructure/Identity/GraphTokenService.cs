using Microsoft.Extensions.Options;
using System.Text.Json;
using UserService.Configuration;

namespace UserService.Infrastructure.Identity
{
    /// <summary>
    /// GraphTokenService handles secure token acquisition for Microsoft Graph using the client credentials flow.
    /// It requests an app-only token from Microsoft Entra ID and caches it until expiry to avoid repeated calls.
    /// </summary>
    public class GraphTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly EntraExternalIdSettings _entraExternalIdSettings;
        private readonly ILogger<GraphTokenService> _logger;

        // 🧠 Cached token and its expiry time
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public DateTime TokenExpiry => _tokenExpiry;

        /// <summary>
        /// Constructor injects HttpClient, options for reading EntraExternalIdSettings from appSettings.json, and logger.
        /// </summary>
        public GraphTokenService(HttpClient httpClient, IOptions<EntraExternalIdSettings> options, ILogger<GraphTokenService> logger)
        {
            _httpClient = httpClient;
            _entraExternalIdSettings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a valid access token for Microsoft Graph.
        /// If a cached token exists and is still valid, it returns that.
        /// Otherwise, it requests a new token from Entra ID.
        /// </summary>
        public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            // ✅ Step 1: Return cached token if it's still valid
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            // ✅ Step 2: Read Entra credentials from configuration
            var tenantId = _entraExternalIdSettings.TenantId;
            var clientId = _entraExternalIdSettings.ClientId;
            var clientSecret = _entraExternalIdSettings.ClientSecret;

            // fail fast if config is misbound
            if (string.IsNullOrWhiteSpace(tenantId) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret))
            {
                _logger.LogError("Missing required EntraExternalIdSettings values.");
                return null;
            }


            // ✅ Step 3: Build the token request URL
            var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            // ✅ Step 4: Prepare the request body for client_credentials flow
            var tokenBody = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", "https://graph.microsoft.com/.default" },
                { "grant_type", "client_credentials" }
            };

            try
            {
                // ✅ Step 5: Send the token request to Microsoft Identity Platform
                var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(tokenBody), cancellationToken);

                // ❌ Step 6: If request fails, log and return null
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to acquire Graph token. Status: {Status}", response.StatusCode);
                    return null;
                }

                // ✅ Step 7: Parse the token response
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var doc = JsonDocument.Parse(json).RootElement;

                // ✅ Step 8: Extract and cache the access token
                _cachedToken = doc.GetProperty("access_token").GetString();

                // ✅ Step 9: Calculate expiry time and subtract buffer (60 seconds)
                var expiresIn = doc.GetProperty("expires_in").GetInt32();
                _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

                // ✅ Step 10: Return the token
                return _cachedToken;
            }
            catch (Exception ex)
            {
                // 🧨 Step 11: Log unexpected errors and return null
                _logger.LogError(ex, "Error requesting Graph token");
                return null;
            }
        }
    }
}

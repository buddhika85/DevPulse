using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Configuration.jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserService.Configuration;

namespace UserService.Services
{
    /// <summary>
    /// Entra does not know user role, so entra issued token does not contain user role.
    /// So we need to issue a Dev Pulse JWT token to perform Role Based Authorisation with in Dev Pulse microservices.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenService> _logger;
        private readonly EntraExternalIdSettings _entraExternalIdSettings;
        private readonly DevPulseJwtSettings _devPulseJwtSettings;

        public TokenService(IHttpClientFactory httpClientFactory, ILogger<TokenService> logger, IOptions<EntraExternalIdSettings> optionsEntra, IOptions<DevPulseJwtSettings> optionsJwt)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _entraExternalIdSettings = optionsEntra.Value;
            _devPulseJwtSettings = optionsJwt.Value;
        }

        /// <summary>
        /// Genarates Dev Pulse JWT to use with in Dev Pulse micro services.
        /// Why we create it? 
        /// Because Entra does not know user role, so entra issued token does not have user role.
        /// So we need to create a Dev Pulse JWT token to perform Role Based Authorisation with Dev Pulse microservices.
        /// </summary>
        /// <param name="entraClaims">claims coming from Entra</param>
        /// <param name="role">User role stored in User DB</param>
        /// <param name="cancellationToken">For cacelling in the middle of the process</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if entraClaims are empty</exception>
        /// <exception cref="ArgumentException">if role is empty</exception>
        public string GenerateAppToken(ClaimsPrincipal entraClaims, string role, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Defensive checks: ensure claims and role are valid
                if (entraClaims == null)
                {
                    _logger.LogWarning("GenerateAppToken called with null ClaimsPrincipal.");
                    throw new ArgumentNullException(nameof(entraClaims));
                }

                // extra entra object Id of the user
                var objectId = entraClaims.FindFirst("oid")?.Value ?? entraClaims.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
                if (string.IsNullOrWhiteSpace(objectId))
                {
                    _logger.LogWarning("GenerateAppToken called with null entra object ID.");
                    throw new ArgumentNullException(nameof(entraClaims));
                }

                var email = entraClaims.FindFirst("email")?.Value ?? 
                    entraClaims.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value ??            // - User Principal Name (UPN)
                    entraClaims.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;             // - username / login name


                // roles valid?
                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogError("GenerateAppToken called with empty role.");
                    throw new ArgumentException("Role cannot be null or empty.", nameof(role));
                }

                // email clauim available?
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogError("GenerateAppToken called with empty email claim.");
                    throw new ArgumentException("Email cannot be null or empty.", nameof(email));
                }

                // 🔑 Create signing key from configuration (symmetric key for demo; use Key Vault in prod)
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_devPulseJwtSettings.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // 📋 Build claims: user ID, email, and application role
                var claims = new List<Claim>
                {
                    new (JwtRegisteredClaimNames.Sub, objectId),                                                        // Subject claim (standard JWT field) set to Entra object ID
                    new ("oid", objectId),                                                                              // Explicit Entra object ID claim for easy access
                    new (JwtRegisteredClaimNames.Email, email),                                                         // Email claim
                    new ("role", role)                                                                                  // Application role claim
                };

                // 🕒 Set expiry based on configuration
                var expiry = DateTime.UtcNow.AddHours(_devPulseJwtSettings.ExpiryHours);

                // 🛠 Construct JWT with issuer, audience, claims, expiry, and signing credentials
                var token = new JwtSecurityToken(
                    issuer: _devPulseJwtSettings.Issuer,
                    audience: _devPulseJwtSettings.Audiences.FirstOrDefault() ?? "DevPulseClient",
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                // 📝 Serialize token to string
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                // 📌 Log success with user ID, role, and expiry
                _logger.LogInformation("App token generated successfully for user {UserId} with role {Role}, expires at {Expiry}.",
                    entraClaims.FindFirst("oid")?.Value,
                    role,
                    expiry);

                return jwt;
            }
            catch (Exception ex)
            {
                // ❌ Log error with exception details
                _logger.LogError(ex, "Failed to generate app token for role {Role}.", role);
                throw; // rethrow so caller can handle appropriately
            }
        }


        /// <summary>
        /// Validates an Entra-issued token. 
        /// This token does not contain user role claims and is only used 
        /// before issuing an application-specific token enriched with roles.
        /// </summary>
        /// <param name="entraToken">The raw Entra JWT access token</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>A ClaimsPrincipal if validation succeeds, otherwise null</returns>
        public async Task<ClaimsPrincipal?> ValidateEntraTokenAsync(string entraToken, CancellationToken cancellationToken)
        {
            // 📝 Log start of validation with timestamp
            _logger.LogInformation("Starting validation of Entra token at {Time}", DateTime.UtcNow);

            // ✅ Create token handler
            var handler = new JwtSecurityTokenHandler();

            // 🔑 Build validation parameters using Entra settings
            var validationParams = new TokenValidationParameters
            {
                // Expected issuer (tenant-specific)
                ValidIssuer = $"{_entraExternalIdSettings.Instance}{_entraExternalIdSettings.TenantId}/v2.0",

                // Expected audiences (from configuration)
                ValidAudiences = _entraExternalIdSettings.Audiences,

                // Public signing keys fetched from Entra JWKS endpoint
                IssuerSigningKeys = await GetEntraSigningKeysAsync(cancellationToken),

                // Extra safety checks
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

            try
            {
                // 🛠 Validate token against parameters
                var principal = handler.ValidateToken(entraToken, validationParams, out _);

                // 📌 Log success with subject claim (user ID)
                _logger.LogInformation("Entra token validated successfully for user {UserId} at {Time}.",
                    principal.FindFirst("oid")?.Value,
                    DateTime.UtcNow);

                return principal;
            }
            catch (Exception ex)
            {
                // ❌ Log error with exception details
                _logger.LogError(ex, "Error while validating Entra issued token at {Time}", DateTime.UtcNow);
                return null;
            }
        }

        /// <summary>
        /// Retrieves Entra ID public signing keys (JSON Web Key Set, JWKS).
        /// These keys are rotated periodically by Entra ID, so they must be fetched via HTTP.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>A collection of SecurityKeys used to validate Entra-issued JWTs</returns>
        private async Task<IEnumerable<SecurityKey>> GetEntraSigningKeysAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Entra ID JWKS at {Time}", DateTime.UtcNow);

                // 🔹 Build JWKS endpoint URL
                var client = _httpClientFactory.CreateClient("EntraKeys");
                var jwksUrl = $"{client.BaseAddress}discovery/v2.0/keys";

                _logger.LogInformation("JWKS Endpoint: {DiscoveryEndpoint}", jwksUrl);

                // 🔹 Fetch JWKS JSON
                var response = await client.GetStringAsync("discovery/v2.0/keys", cancellationToken);

                // 🔹 Parse JWKS into SecurityKeys
                var jwks = new JsonWebKeySet(response);

                _logger.LogInformation("Received {JwksKeyCount} signing keys from Entra ID", jwks.Keys.Count);

                return jwks.Keys;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching Entra ID JWKS");
                throw new ApplicationException("Failed to fetch JWKS from Entra ID", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for Entra ID JWKS");
                throw new ApplicationException("Failed to deserialize JWKS from Entra ID", ex);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("JWKS fetch operation was canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception while fetching Entra ID JWKS");
                throw;
            }
        }

    }
}

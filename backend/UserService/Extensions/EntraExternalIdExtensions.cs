using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Configuration.jwt;
using System.Text;
using UserService.Configuration;

namespace UserService.Extensions
{
    public static class EntraExternalIdExtensions
    {
        /// <summary>
        /// Injects Microsoft Entra External ID JWT authentication using options pattern.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">App configuration</param>
        /// <returns>Configured service collection</returns>
        /// <exception cref="InvalidOperationException">If config section is missing</exception>
        public static IServiceCollection InjectEntraExternalIdAccessService(this IServiceCollection services, IConfiguration configuration)
        {
            // Authentication Scheme 1 - Validate Microsfot Az Entra issued token
            ConfigureEntraJwtValidation(services, configuration);

            // Authentication Scheme 2 - Validate DevPulse issued token containing user role
            ConfigureDevPulseJwtValidation(services, configuration);

            return services;
        }

        // Authentication Scheme 1 - Validate Microsfot Az Entra issued token
        // used only for user service /me endpoint 
        // On /me endpoint → decorate with [Authorize(AuthenticationSchemes = "EntraJwt")]
        private static void ConfigureEntraJwtValidation(IServiceCollection services, IConfiguration configuration)
        {
            // Retrieves the bound EntraExternalIdSettings object directly from configuration.
            var entraSettingsSection = configuration.GetSection("EntraExternalIdSettings");



            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (!entraSettingsSection.Exists())
                throw new InvalidOperationException("EntraExternalIdSettings section is missing or invalid.");

            var audiences = entraSettingsSection.GetSection("Audiences").Get<string[]>();


            // authentication scheme 1
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("EntraJwt", options =>
                {
                    var tenantId = entraSettingsSection["TenantId"];
                    var instance = entraSettingsSection["Instance"];


                    options.Authority = $"{instance}{tenantId}/v2.0";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = audiences,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        NameClaimType = "name",

                        // ✅ Accept both v1 and v2 issuers
                        ValidIssuers =
                        [
                            $"https://login.microsoftonline.com/{tenantId}/v2.0",           // v2 
                            $"https://sts.windows.net/{tenantId}/"                          // v1 
                        ]
                    };
                });

            // adding authorisation policies
            // [Authorize(Policy = "ValidToken")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ValidToken", policy =>
                {
                    // This ensures that any token with a valid signature and lifetime is accepted — even if it lacks scope or roles.
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("http://schemas.microsoft.com/identity/claims/objectidentifier");
                });
            });
        }

        // Authentication Scheme 2 - Validate DevPulse issued token containing user role
        // used by all other authorised endpoints except for user service /me endpoint
        // On all other endpoints → decorate with [Authorize(AuthenticationSchemes = "DevPulseJwt")]
        private static void ConfigureDevPulseJwtValidation(IServiceCollection services, IConfiguration configuration)
        {
            // Retrieves the bound DevPulseJwtSettings object directly from configuration.
            var devPulseJwtSection = configuration.GetSection("DevPulseJwtSettings");



            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (!devPulseJwtSection.Exists())
                throw new InvalidOperationException("DevPulseJwtSettings section is missing or invalid.");

            var issuer = devPulseJwtSection["Issuer"];
            var key = devPulseJwtSection["Key"];
            var audiences = devPulseJwtSection.GetSection("Audiences").Get<string[]>();

            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(key) || audiences == null || !audiences.Any())
                throw new InvalidOperationException("DevPulseJwtSettings section is incomplete.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("DevPulseJwt", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audiences.FirstOrDefault() ?? "DevPulseClient",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };

                });
        }

        /// <summary>
        /// Binds EntraExternalIdSettings from configuration using the Options pattern.
        /// This makes the settings injectable via IOptions<EntraExternalIdSettings>.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The app configuration</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection BindEntraExternalIdSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ Bind the "EntraExternalIdSettings" section to a strongly-typed class
            services.Configure<EntraExternalIdSettings>(
                configuration.GetSection("EntraExternalIdSettings"));

            return services;
        }


        /// <summary>
        /// Binds DevPulseJwtSettings from configuration using the Options pattern.
        /// This makes the settings injectable via IOptions<DevPulseJwtSettings>.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The app configuration</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection BindJwtSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ Bind the "EntraExternalIdSettings" section to a strongly-typed class
            services.Configure<DevPulseJwtSettings>(
                configuration.GetSection("DevPulseJwtSettings"));

            return services;
        }
    }
}

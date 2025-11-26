using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedLib.Configuration.jwt
{
    public static class JwtExtensions
    {
        /// <summary>
        /// Injects JWT authentication logic using options pattern.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">App configuration</param>
        /// <returns>Configured service collection</returns>
        /// <exception cref="InvalidOperationException">If config section is missing</exception>
        public static IServiceCollection InjectDevPulseJwtValidationService(this IServiceCollection services, IConfiguration configuration)
        {
            // Authentication Scheme 1 - Validate Microsfot Az Entra issued token
            // used only for user service /me endpoint 
            // On /me endpoint → decorate with [Authorize(AuthenticationSchemes = "EntraJwt")]
            // This is not available here as it is only used by Users API which direclty intaracts with Mic Entra External ID

            // Authentication Scheme 2 - Validate DevPulse issued token containing user role
            ConfigureDevPulseJwtValidation(services, configuration);

            return services;
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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
            // Retrieves the bound EntraExternalIdSettings object directly from configuration.
            var entraSettingsSection = configuration.GetSection("EntraExternalIdSettings");
            


            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (!entraSettingsSection.Exists())
                throw new InvalidOperationException("EntraExternalIdSettings section is missing or invalid.");

            var audiences = entraSettingsSection.GetSection("Audiences").Get<string[]>();

            

            // Register Az Microsoft Entra External Id Access service           
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
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


            return services;
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

    }
}

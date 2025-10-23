using Microsoft.Identity.Web;

namespace UserService.Extensions
{
    public static class AzureActiveDirB2CExtensions
    {
        /// <summary>
        /// Options Pattern to bind Azure AD B2C settings from configuration
        /// </summary>
        /// <param name="services">services collection</param>
        /// <param name="configuration">To read from settings</param>
        /// <returns>AzureActiveDirB2C added services collection</returns>
        /// <exception cref="InvalidOperationException">if AzureAdB2CSettings invalid or do not exist</exception>
        public static IServiceCollection InjectAzureAdB2CAccessService(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieves the bound AzureAdB2CSettings object directly from configuration.
            var azB2CSettingsSection = configuration.GetSection("AzureAdB2CSettings");
            

            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (!azB2CSettingsSection.Exists())
                throw new InvalidOperationException("AzureAdB2CSettings section is missing or invalid.");

            // Register Az B2C Access service
            services.AddAuthentication("Bearer").AddMicrosoftIdentityWebApi(azB2CSettingsSection);
            return services;
        }
    }
}

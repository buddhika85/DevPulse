using UserService.Configuration;

namespace UserService.Extensions
{
    public static class HttpClientPolicies
    {
        public static IServiceCollection AddHttpClientPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieves the bound EntraExternalIdSettings object directly from configuration.
            var entraSettingsSection = configuration.GetSection("EntraExternalIdSettings");

            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (!entraSettingsSection.Exists())
                throw new InvalidOperationException("EntraExternalIdSettings section is missing or invalid.");

            var tenantId = entraSettingsSection["TenantId"];
            var instance = entraSettingsSection["Instance"];

            services.AddHttpClient("EntraKeysClient", client => 
            {
                client.BaseAddress = new Uri($"{instance}{tenantId}/");
            });

            return services;
        }
    }
}

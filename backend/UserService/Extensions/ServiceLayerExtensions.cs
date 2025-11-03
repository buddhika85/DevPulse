using UserService.Infrastructure.Identity;
using UserService.Services;


namespace UserService.Extensions
{
    public static class ServiceLayerExtensions
    {
        /// <summary>
        /// Registers services with DI container.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="loggerFactory">Factory used to create a logger for diagnostics.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection InjectServices(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddScoped<IUserService, Services.UserService>();


            // External API calls with resilience and reuse (its transient) - To Micro Az Entra Extrenal ID for token related workflows
            services.AddHttpClient<IExternalIdentityProvider, EntraIdentityProvider>();

            // transient Http Client with GraphTokenService
            services.AddHttpClient<GraphTokenService>();

            return services;
        }
    }
}

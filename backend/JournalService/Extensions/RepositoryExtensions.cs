using JournalService.Repositories;

namespace JournalService.Extensions
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Registers repositories with DI container.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="loggerFactory">Factory used to create a logger for diagnostics.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection InjectRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJournalRepository, JournalRepository>();
            services.AddScoped<IJournalFeedbackRepository, JournalFeedbackRepository>();
            return services;
        }
    }
}

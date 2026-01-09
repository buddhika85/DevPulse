using OrchestratorService.Application.Services;
using OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService;

namespace OrchestratorService.Extensions
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
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IJournalService, JournalService>();
            services.AddScoped<ITaskJournalLinkService, TaskJournalLinkService>();
            return services;
        }
    }
}

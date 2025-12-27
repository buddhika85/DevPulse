using JournalService.Infrastructure.Persistence.CosmosEvents;

namespace JournalService.Extensions
{
    public static class CosmosDbServiceExtensions
    {
        public static IServiceCollection InjectCosmosDbServices(this IServiceCollection services)
        {
            services.AddScoped<JournalCosmosEventService>();               // inject MoodCosmosEventService for logging user related events like created, updated...
            return services;
        }
    }
}

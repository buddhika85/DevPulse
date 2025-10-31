using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Extensions
{
    public static class CosmosDbServiceExtensions
    {
        public static IServiceCollection InjectCosmosDbServices(this IServiceCollection services)
        {
            services.AddScoped<UserCosmosEventService>();               // inject UserCosmosEventService for logging user related events like created, updated...
            return services;
        }
    }
}

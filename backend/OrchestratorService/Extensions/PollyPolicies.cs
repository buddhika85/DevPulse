using OrchestratorService.Configurations;
using OrchestratorService.Infrastructure.HttpClients;
using OrchestratorService.Infrastructure.HttpClients.JournalMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using Polly;


namespace OrchestratorService.Extensions
{
    public static class PollyPolicies
    {
        public static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();                                                  // HTTP conex access
            services.AddTransient<JwtForwardingHandler>();                                      // register JwtForwardingHandler

            var microServiceUrls = ReadMicroserviceUrls(services, configuration);
            var pollyConfig = ReadPollyConfig(services, configuration);

            AddUserMicroServicePolicy(services, microServiceUrls, pollyConfig);
            AddTaskMicroServicePolicy(services, microServiceUrls, pollyConfig);
            AddJournalMicroServicePolicy(services, microServiceUrls, pollyConfig);
            AddTaskJounralLinklMicroServicePolicy(services, microServiceUrls, pollyConfig);
            return services;
        }

        private static void AddTaskJounralLinklMicroServicePolicy(IServiceCollection services, MicroServicesUrlSettings microServiceUrls, PollyConfig pollyConfig)
        {
            services.AddHttpClient<ITaskJournalLinkServiceClient, TaskJournalLinkServiceClient>(client =>
            {
                client.BaseAddress = new Uri(microServiceUrls.TaskJournalLinkAPI);
            })
            .AddHttpMessageHandler<JwtForwardingHandler>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(pollyConfig.RetryCount, _ => TimeSpan.FromMilliseconds(pollyConfig.SleepDurationMilliSeconds)));
        }

        private static void AddJournalMicroServicePolicy(IServiceCollection services, MicroServicesUrlSettings microServiceUrls, PollyConfig pollyConfig)
        {
            services.AddHttpClient<IJournalServiceClient, JournalServiceClient>(client =>
            {
                client.BaseAddress = new Uri(microServiceUrls.JounralAPI);
            })
            .AddHttpMessageHandler<JwtForwardingHandler>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(pollyConfig.RetryCount, _ => TimeSpan.FromMilliseconds(pollyConfig.SleepDurationMilliSeconds)));
        }

        private static void AddTaskMicroServicePolicy(IServiceCollection services, MicroServicesUrlSettings microServiceUrls, PollyConfig pollyConfig)
        {
            // TaskService
            services.AddHttpClient<ITaskServiceClient, TaskServiceClient>(client =>
            {
                client.BaseAddress = new Uri(microServiceUrls.TaskAPI);
            })
            .AddHttpMessageHandler<JwtForwardingHandler>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(pollyConfig.RetryCount, _ => TimeSpan.FromMilliseconds(pollyConfig.SleepDurationMilliSeconds)));
        }

        private static void AddUserMicroServicePolicy(IServiceCollection services, MicroServicesUrlSettings microServiceUrls, PollyConfig pollyConfig)
        {
            // UserService
            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(microServiceUrls.UserAPI);
            })
            .AddHttpMessageHandler<JwtForwardingHandler>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(pollyConfig.RetryCount, _ => TimeSpan.FromMilliseconds(pollyConfig.SleepDurationMilliSeconds)));
        }

        /// <summary>
        /// Use options pattern and reads polly configurations from app Settings.json and binds to PollyConfig object and returns
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">PollyConfig of App Settings . json is invalid</exception>
        private static PollyConfig ReadPollyConfig(IServiceCollection services, IConfiguration configuration)
        {
            // Maps the "PollyConfig" section from appsettings.json to the strongly typed MicroServicesUrlSettings class.      
            services.Configure<PollyConfig>(configuration.GetSection("PollyConfig"));

            // Retrieves the bound MicroServicesUrlSettings object directly from configuration.            
            var pollyConfig = configuration.GetSection("PollyConfig").Get<PollyConfig>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (pollyConfig is null)
                throw new InvalidOperationException("PollyConfig section is missing or invalid.");

            return pollyConfig;
        }

        /// <summary>
        /// Use options pattern and reads microServiceUrls from app Settings.json and binds to MicroServicesUrlSettings object and returns
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">MicroServiceUrls of App Settings . json is invalid</exception>
        private static MicroServicesUrlSettings ReadMicroserviceUrls(IServiceCollection services, IConfiguration configuration)
        {
            // Maps the "MicroServiceUrls" section from appsettings.json to the strongly typed MicroServicesUrlSettings class.      
            services.Configure<MicroServicesUrlSettings>(configuration.GetSection("MicroServiceUrls"));

            // Retrieves the bound MicroServicesUrlSettings object directly from configuration.            
            var microServiceUrls = configuration.GetSection("MicroServiceUrls").Get<MicroServicesUrlSettings>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (microServiceUrls is null)
                throw new InvalidOperationException("MicroServiceUrls section is missing or invalid.");

            return microServiceUrls;
        }
    }
}

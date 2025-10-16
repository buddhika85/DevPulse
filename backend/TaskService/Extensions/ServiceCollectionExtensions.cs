using Microsoft.EntityFrameworkCore;
using TaskService.Configuration;
using TaskService.Infrastructure.Persistence;
using TaskService.Repositories;
using TaskService.Services;

namespace TaskService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            SetupDbContext(services, configuration);
            SetupRepositories(services);
            SetupServices(services);
            return services;
        }      

        private static void SetupDbContext(IServiceCollection services, IConfiguration configuration)
        {
            // Maps the "DatabaseSettings" section from appsettings.json to the strongly typed DatabaseSettings class.          
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

            // Retrieves the bound DatabaseSettings object directly from configuration.            
            var dbSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

            // Defensive check: ensures the section exists and is properly bound.
            // If null: fails fast during startup.
            if (dbSettings is null)
                throw new InvalidOperationException("DatabaseSettings section is missing or invalid.");


            // Registers TaskDbContext with dependency injection.
            // Configures EF Core to use SQL Server with the connection string from DatabaseSettings.
            services.AddDbContext<TaskDbContext>(options =>
                options.UseSqlServer(dbSettings.ConnectionString));
        }

        private static void SetupRepositories(IServiceCollection services)
        {
            services.AddScoped<ITaskRepository, TaskRepository>();               // Registers TaskRepository for ITaskRepository
        }

        private static void SetupServices(IServiceCollection services)
        {
            services.AddScoped<ITaskService, Services.TaskService>();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SharedLib.Configuration.databaseConfig;
using UserService.Infrastructure.Persistence;

namespace UserService.Extensions
{

    public static class DbContextExtensions
    {
        /// <summary>
        /// Registers TaskDbContext using SQL Server and binds DatabaseSettings from configuration.
        /// Logs the registration process and connection string for diagnostics.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
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
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(dbSettings.ConnectionString));

            return services;
        }
    }
}

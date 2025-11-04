
using Microsoft.Extensions.Options;
using UserService.Configuration;

namespace UserService.Extensions
{
    public static class CorsExtensions
    {
        private const string PolicyName = "DevPulseCors";

        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ Register CorsSettings from configuration
            services.Configure<CorsSettings>(configuration.GetSection("Cors"));

            // ✅ Temporarily build a provider to resolve options
            using var sp = services.BuildServiceProvider();
            var options = sp.GetRequiredService<IOptions<CorsSettings>>().Value;

            services.AddCors(cors =>
            {
                cors.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins(options.AllowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            return services;
        }

        public static IApplicationBuilder UseConfiguredCors(this IApplicationBuilder app)
        {
            app.UseCors(PolicyName);
            return app;
        }
    }

}

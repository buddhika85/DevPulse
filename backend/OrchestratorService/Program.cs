using Microsoft.AspNetCore.Mvc;
using OrchestratorService.Extensions;
using OrchestratorService.Infrastructure.HttpClients;
using Serilog;
using SharedLib.Configuration.Cors;
using SharedLib.Configuration.jwt;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddConfiguredCors(builder.Configuration);                          // CORS config added

builder.Host.AddSerilogLogging(builder.Services, builder.Configuration);                                           // Serilog logging

// Add services to the container.
builder.Services.AddControllers(options =>                                  // Enables controller routing
{
    options.Filters.Add(new ProducesAttribute("application/json"));         // All controllers return media type JSON explicityly - Ensures consistent content negotiation and Swagger documentation
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>                                   // Swagger generation
{
    options.EnableAnnotations();                                            // Enables [SwaggerOperation] and related attributes
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "DevPulse Orchestrator API",
        Description = "API for managing aggregated API calls in DevPulse within microservices."
    });
});


builder.Services.AddMemoryCache();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache();                                              // Enables response-level caching - Full Action Result
builder.Services.AddMemoryCache();                                              // Enables data-level caching  - UserAccountDto



builder.Services.BindJwtSettings(builder.Configuration);                            // Binds DevPulseJwtSettings from configuration using the Options pattern.
builder.Services.InjectDevPulseJwtValidationService(builder.Configuration);         // inject DevPulse user API issued JWT (not entra issued JWT)


// HttpClientFactory with Polly
builder.Services.AddPollyPolicies(builder.Configuration);

// inject orchestration service
builder.Services.InjectServices();


var app = builder.Build();



// Configure middleware
app.UseExceptionHandler("/error");                                          // If an Unhandled Exception Occurs - Anywhere in the pipeline: controller, service, handler, etc. ASP.NET Core UseExceptionHandler catches it,
                                                                            // - and redirects to /error endpint (inside ErrorController)
app.UseStatusCodePages();                                                   // if 404s and other status codes handled - has no body, it generates a simple text or HTML message - Status Code: 404, Content not found



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();                                                       // Serves Swagger JSON
    app.UseSwaggerUI(x =>                                                   // Serves Swagger UI
    {
        x.ConfigObject.AdditionalItems["showExtensions"] = true;
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "DevPulse Orchestrator API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseConfiguredCors();                                                    // using CORS configurations with CORS middleware

app.UseOutputCache();                                   // Activates output caching middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();




#region StartingApplication


// starting application
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "OrchestratorService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

#endregion StartingApplication
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLib.Application.Behaviors;
using TaskService.Application.Validators;
using TaskService.Extensions;
using TaskService.Infrastructure.Persistence;
using SharedLib.Configuration.Cors;
using SharedLib.Configuration.jwt;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguredCors(builder.Configuration);                          // CORS config added

builder.Host.AddSerilogLogging(builder.Services, builder.Configuration);                                           // Serilog logging



// Register services
builder.Services.AddControllers(options =>                                  // Enables controller routing
{
    options.Filters.Add(new ProducesAttribute("application/json"));         // All controllers return media type JSON explicitly - Ensures consistent content negotiation and Swagger documentation
})
.AddJsonOptions(options =>
{
    // Register custom converters for System.Text.Json
    // This ensures UserRole is serialized/deserialized correctly
    options.JsonSerializerOptions.Converters.Add(
        new SharedLib.Domain.ValueObjects.Converters.UserRoleJsonConverter()
    );
});
builder.Services.AddEndpointsApiExplorer();                                 // Required for Swagger
builder.Services.AddSwaggerGen(options =>                                   // Swagger generation
{
    options.EnableAnnotations();                                            // Enables [SwaggerOperation] and related attributes
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "DevPulse Task API",
        Description = "API for managing developer tasks in DevPulse microservice."
    });
});                                           


builder.Services.AddMediatR(cfg =>                                          // MediatR for CQRS pattern - considers all MediatR handlers (commands, queries) are in the same project as Program.cs.
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddMediatR(typeof(CreateTaskHandler).Assembly);


builder.Services.AddValidatorsFromAssemblyContaining<UpdateTaskDtoValidator>();                 // This auto-registers all Fluent Validators (for DTO, command, and query validators) in the assembly for dependency injection.
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));     // Add MediatR pipeline behavior for Validations -  Allows MediatR to intercept all incoming request (query, command or DTO) and run Fluent validators which were attached to them - If fails throws RequestValidationException


builder.Services.BindJwtSettings(builder.Configuration);                            // Binds DevPulseJwtSettings from configuration using the Options pattern.
builder.Services.InjectDevPulseJwtValidationService(builder.Configuration);         // inject DevPulse user API issued JWT (not entra issued JWT)


builder.Services.InjectDbContext(builder.Configuration);                    // inject DB Context
builder.Services.InjectRepositories(builder.Configuration);                 // inject Repositories
builder.Services.InjectServices(builder.Configuration);                     // inject Services





var app = builder.Build();






// Configure middleware
app.UseExceptionHandler("/error");                                          // If an Unhandled Exception Occurs - Anywhere in the pipeline: controller, service, handler, etc. ASP.NET Core UseExceptionHandler catches it,
                                                                            // - and redirects to /error endpint (inside ErrorController)
app.UseStatusCodePages();                                                   // if 404s and other status codes handled - has no body, it generates a simple text or HTML message - Status Code: 404, Content not found



if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();                                                       // Serves Swagger JSON
    app.UseSwaggerUI(x =>                                                   // Serves Swagger UI
    {
        x.ConfigObject.AdditionalItems["showExtensions"] = true;
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "DevPulse Task API v1");
    });                                                     
}

app.UseHttpsRedirection();                                                  // Enforces HTTPS

app.UseRouting();

app.UseConfiguredCors();                                                    // using CORS configurations with CORS middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();                                                       // Maps controller endpoints



// seeding the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.Migrate();                                                  // ✅ Apply pending migrations before seeding
    DbInitializer.Seed(db);
}






// starting application
try
{
    app.Run();                                                                  
}
catch (Exception ex)
{
    Log.Fatal(ex, "TaskService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


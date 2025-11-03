using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using UserService.Application.Common.Behaviors;
using UserService.Application.Validators;
using UserService.Extensions;
using UserService.Infrastructure.Persistence;






var builder = WebApplication.CreateBuilder(args);

// This disables ASP.NET Core’s automatic remapping of claims like oid, sub, email, etc., to legacy .NET types.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


builder.Host.AddSerilogLogging(builder.Services, builder.Configuration);                                           // Serilog logging



// Register services
builder.Services.AddControllers(options =>                                  // Enables controller routing
{
    options.Filters.Add(new ProducesAttribute("application/json"));         // All controllers return media type JSON explicitly - Ensures consistent content negotiation and Swagger documentation
});
builder.Services.AddEndpointsApiExplorer();                                 // Required for Swagger
builder.Services.AddSwaggerGen(options =>                                   // Swagger generation
{
    options.EnableAnnotations();                                            // Enables [SwaggerOperation] and related attributes
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "DevPulse User API",
        Description = "API for managing user authentication and authorisation in DevPulse microservice suite."
    });
});


builder.Services.AddMediatR(cfg =>                                          // MediatR for CQRS pattern - considers all MediatR handlers (commands, queries) are in the same project as Program.cs.
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddMediatR(typeof(CreateTaskHandler).Assembly);


builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();                   // This auto-registers all Fluent Validators (for DTO, command, and query validators) in the assembly for dependency injection.
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));             // Add MediatR pipeline behavior for Validations -  Allows MediatR to intercept all incoming request (query, command or DTO) and run Fluent validators which were attached to them - If fails throws RequestValidationException

builder.Services.InjectDbContext(builder.Configuration);                    // inject DB Context
builder.Services.InjectRepositories(builder.Configuration);                 // inject Repositories
builder.Services.InjectServices(builder.Configuration);                     // inject Services


builder.Services.BindEntraExternalIdSettings(builder.Configuration);                // Binds EntraExternalIdSettings from configuration using the Options pattern.


builder.Services.InjectEntraExternalIdAccessService(builder.Configuration);         // inject Azure Microsoft Entra External Id for JWT auth, author
builder.Services.InjectCosmosDbServices();                                          // inject services which logs to Azure Cosmos DB 


var app = builder.Build();






// Configure middleware
app.UseExceptionHandler("/error");                                          // If an Unhandled Exception Occurs - Anywhere in the pipeline: controller, service, handler, etc. ASP.NET Core UseExceptionHandler catches it,
                                                                            // - and redirects to /error endpint (inside ErrorController)
app.UseStatusCodePages();                                                   // if 404s and other status codes handled - has no body, it generates a simple text or HTML message - Status Code: 404, Content not found



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                                                       // Serves Swagger JSON
    app.UseSwaggerUI(x =>                                                   // Serves Swagger UI
    {
        x.ConfigObject.AdditionalItems["showExtensions"] = true;
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "DevPulse Task API v1");
    });
}

app.UseHttpsRedirection();                                                  // Enforces HTTPS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();                                                       // Maps controller endpoints



// seeding the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
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


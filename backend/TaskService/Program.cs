using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using TaskService.Extensions;
using TaskService.Infrastructure.Persistence;





var builder = WebApplication.CreateBuilder(args);


builder.Host.AddSerilogLogging(builder.Services, builder.Configuration);                                           // Serilog logging



// Register services
builder.Services.AddControllers(options =>                                  // Enables controller routing
{
    options.Filters.Add(new ProducesAttribute("application/json"));         // All controllers return media type JSON explicityly - Ensures consistent content negotiation and Swagger documentation
});                                          
builder.Services.AddEndpointsApiExplorer();                                 // Required for Swagger
builder.Services.AddSwaggerGen(options =>                                   // Swagger generation
{
    options.EnableAnnotations();                                            // 👈 Enables [SwaggerOperation] and related attributes
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



builder.Services.InjectDbContext(builder.Configuration);                    // inject DB Context
builder.Services.InjectRepositories(builder.Configuration);                 // inject Repositories
builder.Services.InjectServices(builder.Configuration);                     // inject Services





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
app.MapControllers();                                                       // Maps controller endpoints



// seeding the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
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


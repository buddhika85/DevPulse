using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddSwaggerGen();                                           // Swagger generation


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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                                                       // Serves Swagger JSON
    app.UseSwaggerUI(x =>                                                   // Serves Swagger UI
    {
        x.ConfigObject.AdditionalItems["showExtensions"] = true;

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


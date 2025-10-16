using Serilog;
using TaskService.Extensions;
using TaskService.Infrastructure.Persistence;





var builder = WebApplication.CreateBuilder(args);


builder.Host.AddSerilogLogging(builder.Services, builder.Configuration);                                           // Serilog logging

// Register services
builder.Services.AddControllers();                                          // Enables controller routing
builder.Services.AddEndpointsApiExplorer();                                 // Required for Swagger
builder.Services.AddSwaggerGen();                                           // Swagger generation


builder.Services.AddMediatR(cfg =>                                          // MediatR for CQRS pattern - considers all MediatR handlers (commands, queries) are in the same project as Program.cs.
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddMediatR(typeof(CreateTaskHandler).Assembly);



builder.Services.AddTaskServiceDependencies(builder.Configuration);         // DB Context, Repositories, Services








var app = builder.Build();






// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                                                       // Serves Swagger JSON
    app.UseSwaggerUI(x =>
    {
        x.ConfigObject.AdditionalItems["showExtensions"] = true;

    });                                                     // Serves Swagger UI
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


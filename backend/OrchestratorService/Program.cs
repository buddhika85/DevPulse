using OrchestratorService.Extensions;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache();                                              // Enables response-level caching - Full Action Result
builder.Services.AddMemoryCache();                                              // Enables data-level caching  - UserAccountDto



// HttpClientFactory with Polly
builder.Services.AddPollyPolicies(builder.Configuration);

// inject orchestration service
builder.Services.InjectServices();



var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseOutputCache();                                   // Activates output caching middleware


app.UseAuthorization();

app.MapControllers();

app.Run();

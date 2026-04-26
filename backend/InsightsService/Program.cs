using InsightsService.Data;
using InsightsService.Endpoints;
using InsightsService.GraphQL;
using InsightsService.GraphQL.Types;
using InsightsService.Options;
using InsightsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Options Pattern
builder.Services.Configure<DatabaseConnections>(
    builder.Configuration.GetSection("DatabaseConnections"));

// TO DO: add Cors
// TO DO: Add Seq
// TO DO: Add Serilog loggin... check with other Program.cs files

// Dapper
/* Why Singleton? not scoped like EF Core DB Context?
 DapperContext is registered as Singleton because it is a stateless factory that only holds immutable connection strings and creates new SqlConnection instances per call. 
Transactional integrity is handled by the connection and transaction objects, not by the lifetime of the context. 
Scoped lifetimes are required only when the service holds request-specific state, such as EF Core DbContext.
 */
builder.Services.AddSingleton<DapperContext>();

// For Cosmos queries
builder.Services.AddSingleton<TaskJournalLinkCosmosDb>();

// Insights services
builder.Services.AddScoped<IMoodInsightsService, MoodInsightsService>();
//builder.Services.AddScoped<TaskInsightsService>();
//builder.Services.AddScoped<JournalInsightsService>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<InsightsQuery>()      // GraphQL “Query root” 
    .AddType<MoodStatsType>();          // HotChocolate will automatically use our MoodStatsType whenever a field returns MoodStatsDto.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// add REST route group for consistency
var insights = app.MapGroup("/insights");    // https://localhost:7064/swagger/index.html

// Minimal API endpoints
insights.MapMoodEndpoints();
//insights.MapTaskEndpoints();
//insights.MapJournalEndpoints();

// GraphQL endpoint - Exposed from root 
app.MapGraphQL("/graphql");                 // https://localhost:7064/graphql/

app.Run();

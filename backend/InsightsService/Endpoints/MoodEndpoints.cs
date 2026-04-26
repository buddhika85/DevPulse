using InsightsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsightsService.Endpoints;

public static class MoodEndpoints
{
    public static void MapMoodEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/insights/mood/average/{daysBack:int}", async ([FromRoute]int daysBack, [FromServices] IMoodInsightsService svc) =>
        {
            var result = await svc.GetAverageMoodAsync(daysBack);
            return Results.Ok(result);
        })
        .WithName("GetAverageMood")
        .WithOpenApi();
    }
}

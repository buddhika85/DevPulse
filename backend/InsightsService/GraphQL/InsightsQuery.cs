using InsightsService.Models;
using InsightsService.Services;

namespace InsightsService.GraphQL;

// https://localhost:7064/graphql/
// Resolver class
// A resolver is simply a C# method that HotChocolate calls when a GraphQL field is requested.
//Class name → Query root
public class InsightsQuery
{
    // Method names → GraphQL fields

    [GraphQLDescription("Returns mood statistics for all users over the given number of days.")]
    public Task<IEnumerable<MoodStatsDto>> GetMoodStats(
        [GraphQLDescription("Number of days to look back from today.")]  int daysBack,
        [Service] IMoodInsightsService svc)
        => svc.GetAverageMoodAsync(daysBack);


    //TaskInsights
    //JournalInsights
    //MoodTrend(time‑series)
    //Productivity correlation
}

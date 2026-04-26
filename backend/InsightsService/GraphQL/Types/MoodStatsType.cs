using InsightsService.Models;

namespace InsightsService.GraphQL.Types;

/* This gives us -
- field descriptions
- schema clarity
- better Banana Cake Pop documentation
- full control over naming, nullability, and formatting


MoodStatsType gives you:
human‑readable descriptions
better API discoverability
frontend developers instantly understand the fields
a clean, self‑documenting schema
the ability to rename GraphQL fields without touching C#
*/
public class MoodStatsType : ObjectType<MoodStatsDto>
{
    protected override void Configure(IObjectTypeDescriptor<MoodStatsDto> descriptor)
    {
        descriptor.Description("Aggregated mood statistics for a single user over a given time window.");

        descriptor.Field(x => x.UserId)
            .Description("The ID of the user.");

        descriptor.Field(x => x.DaysBack)
            .Description("Number of days included in this mood analysis.");

        descriptor.Field(x => x.MoodEntriesCount)
            .Description("Total number of mood entries recorded for the user in the given time window.");

        descriptor.Field(x => x.TotalMoodScore)
            .Description("Sum of all mood scores for the user.");

        descriptor.Field(x => x.AvgMoodScore)
            .Description("Average mood score for the user, rounded to two decimal places.");

        descriptor.Field(x => x.AvgMoodString)
            .Description("A human-friendly summary of the user's average mood.");
    }
}

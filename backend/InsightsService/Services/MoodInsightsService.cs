using Dapper;
using InsightsService.Data;
using InsightsService.Models;
using MoodService.Domain.ValueObjects;

namespace InsightsService.Services;

public class MoodInsightsService : IMoodInsightsService
{
    private readonly DapperContext _context;

    public MoodInsightsService(DapperContext context)
    {
        _context = context;
    }

    /*
     Sample output - List of -
        {
          "userId": 123,
          "daysBack": 7,
          "moodEntriesCount": 3,
          "totalMoodScore": 4,
          "avgMoodScore": 1.33,
          "avgMoodString": "Calm"
        }
     */
    public async Task<IEnumerable<MoodStatsDto>> GetAverageMoodAsync(int daysBack)
    {
        using var conn = _context.MoodDb();

        var sql = @"
            SELECT 
                UserId,
                MoodLevel
            FROM MoodEntries
            WHERE Day >= DATEADD(day, -@Days, GETDATE());
        ";

        var rows = await conn.QueryAsync<MoodEntryRow>(sql, new { Days = daysBack });

        var result = from row in rows
                     group row by row.UserId into usersGroup

                     let moodEntryCount = usersGroup.Count()
                     let totalMoodScore = usersGroup.Sum(x => MoodLevel.From(x.MoodLevel).Score)
                     let avgMoodScore = Math.Round(totalMoodScore / (float)moodEntryCount, 2)

                     let roundedScore = (int)Math.Round(avgMoodScore, MidpointRounding.AwayFromZero)
                     let avgMood = MoodLevel.From(roundedScore)

                     select new MoodStatsDto(
                            UserId: usersGroup.Key,
                            DaysBack: daysBack,
                            MoodEntriesCount: moodEntryCount,
                            TotalMoodScore: totalMoodScore,
                            AvgMoodScore: avgMoodScore,
                            AvgMoodString: avgMood.Value
                        );

        return [.. result];
    }
}

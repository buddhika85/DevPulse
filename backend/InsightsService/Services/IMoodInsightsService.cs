using InsightsService.Models;

namespace InsightsService.Services
{
    public interface IMoodInsightsService
    {
        Task<IEnumerable<MoodStatsDto>> GetAverageMoodAsync(int daysBack);
    }
}
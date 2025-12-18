using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;
using SharedLib.Application.Interfaces;

namespace MoodService.Repositories
{
    public interface IMoodRepository : IBaseRepository<MoodEntry>
    {
        Task<IReadOnlyList<MoodEntry>> GetMoodEntriesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> IsMoodEntryExists(Guid userId, DateTime day, MoodTime session, CancellationToken cancellationToken);

        Task<MoodEntry?> FindOtherMoodEntry(Guid excludeId, Guid userId, DateTime day, MoodTime session, CancellationToken cancellationToken);
    }
}

using MoodService.Domain.Entities;
using SharedLib.Application.Interfaces;

namespace MoodService.Repositories
{
    public interface IMoodRepository : IBaseRepository<MoodEntry>
    {
        Task<IReadOnlyList<MoodEntry>> GetMoodEntriesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}

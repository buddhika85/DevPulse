using MoodService.Application.Commands;
using MoodService.Application.Queries;
using SharedLib.DTOs.Mood;

namespace MoodService.Services
{
    public interface IMoodService
    {
        Task<IReadOnlyList<MoodEntryDto>> GetAllMoodEntriesAsync(CancellationToken cancellationToken);
        Task<MoodEntryDto?> GetMoodEntryByIdAsync(GetMoodEntryByIdQuery query, CancellationToken cancellationToken);
        Task<IReadOnlyList<MoodEntryDto>> GetMoodEntriesByUserIdAsync(GetMoodEntriesByUserIdQuery query, CancellationToken cancellationToken);
        Task<bool> IsMoodEntryExists(IsMoodEntryExistsCommand command, CancellationToken cancellationToken);            // before insert
        Task<bool> FindOtherMoodEntry(FindOtherMoodEntryCommand command, CancellationToken cancellationToken);          // before update

        Task<Guid?> AddMoodEntryAsync(AddMoodEntryCommand command, CancellationToken cancellationToken);
        Task<bool> UpdateMoodEntryAsync(UpdateMoodEntryCommand command, CancellationToken cancellationToken);
        Task<bool> DeleteMoodEntryAsync(DeleteMoodEntryCommand command, CancellationToken cancellationToken);        
    }
}

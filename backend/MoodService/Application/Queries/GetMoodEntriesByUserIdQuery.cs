using MediatR;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Queries
{
    public record GetMoodEntriesByUserIdQuery(Guid UserId) : IRequest<IReadOnlyList<MoodEntryDto>>
    {
    }
}

using MediatR;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Queries
{
    public record GetMoodEntriesQuery : IRequest<IReadOnlyList<MoodEntryDto>>
    {
    }
}

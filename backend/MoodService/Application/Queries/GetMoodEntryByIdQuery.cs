using MediatR;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Queries
{
    public record GetMoodEntryByIdQuery(Guid Id) : IRequest<MoodEntryDto?>
    { }
}

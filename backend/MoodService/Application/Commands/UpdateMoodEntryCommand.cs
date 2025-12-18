using MediatR;

namespace MoodService.Application.Commands
{
    public record UpdateMoodEntryCommand(Guid Id,  DateTime Day, string MoodTime, string MoodLevel, string? Note): IRequest<bool>
    {}
}

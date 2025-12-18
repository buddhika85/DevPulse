using MediatR;

namespace MoodService.Application.Commands
{
    public record AddMoodEntryCommand(Guid UserId,
                                        DateTime? Day,
                                        string? MoodTime,
                                        string? MoodLevel,
                                        string? Note): IRequest<Guid?>
    {}
}

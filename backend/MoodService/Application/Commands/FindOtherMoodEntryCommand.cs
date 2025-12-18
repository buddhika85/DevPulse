using MediatR;

namespace MoodService.Application.Commands
{
    public record FindOtherMoodEntryCommand(Guid ExcludeId,
                                        Guid UserId,
                                        DateTime Day,
                                        string MoodTime) : IRequest<bool>
    { }
}

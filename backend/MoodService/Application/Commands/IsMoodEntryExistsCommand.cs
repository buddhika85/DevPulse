using MediatR;

namespace MoodService.Application.Commands
{
    public record IsMoodEntryExistsCommand(Guid UserId,
                                        DateTime Day,
                                        string MoodTime) : IRequest<bool>
    { }
}

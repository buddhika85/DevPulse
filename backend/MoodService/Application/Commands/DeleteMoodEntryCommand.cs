using MediatR;

namespace MoodService.Application.Commands
{
    public record DeleteMoodEntryCommand(Guid Id) : IRequest<bool>
    {}
}

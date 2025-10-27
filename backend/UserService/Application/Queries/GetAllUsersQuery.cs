using MediatR;
using UserService.Application.Dtos;

namespace UserService.Application.Queries
{
    public record GetAllUsersQuery : IRequest<IReadOnlyList<UserAccountDto>>
    {
    }
}

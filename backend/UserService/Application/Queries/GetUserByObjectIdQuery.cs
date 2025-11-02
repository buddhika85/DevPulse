using MediatR;
using UserService.Application.Dtos;

namespace UserService.Application.Queries
{
    public record GetUserByObjectIdQuery(string ObjectId) : IRequest<UserAccountDto>;
}

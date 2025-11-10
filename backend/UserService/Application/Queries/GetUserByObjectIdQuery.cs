using MediatR;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{
    public record GetUserByObjectIdQuery(string ObjectId) : IRequest<UserAccountDto>;
}
